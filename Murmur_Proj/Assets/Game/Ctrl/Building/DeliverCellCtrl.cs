using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class DeliverCellCtrl : BuildingCtrl
{
    private SpotConfig _counter;
    private PuppetCtrl _worker;
    private bool _working;
    private float _timer;
    private List<string> _ingredientTypes = new List<string>();
    private List<string> _takeIngredients = new List<string>();
    private Dictionary<string, SpotConfig> _takeSpots = new Dictionary<string, SpotConfig>();
    private List<DeliverOrder> _deliverOrders = new List<DeliverOrder>();
    private Dictionary<string, int> _requires = new Dictionary<string, int>();
    private Dictionary<string, int> _ingredients = new Dictionary<string, int>();

    private int _takeStep;
    private int _deliverStep;

    public override void InitFlow()
    {
        var prefix = _buildingData.Config.id;
        _counter = ConfigModule.Instance.GetSpot(prefix, SpotDefine.Counter, 1);

        var ingredientTypes = typeof(IngredientType).GetFields(BindingFlags.Static | BindingFlags.Public);
        foreach(var propType in ingredientTypes) {
            var prop = propType.GetValue(null).ToString();
            _ingredientTypes.Add(prop);
            _takeIngredients.Add(prop);
            _takeSpots.Add(prop, ConfigModule.Instance.GetSpot(prop, SpotDefine.Take, 1));
            _requires.Add(prop, 0);
            _ingredients.Add(prop, 0);
        }
        _timer = 0;
        _takeStep = 0;
        _deliverStep = 0;
        RefreshWorker();
        SetWorkIngredients(true);
    }
    public override void RefreshWorker()
    {
        if(_worker != null) return;
        var workerId = _buildingData.GetStaff();
        if(string.IsNullOrEmpty(workerId)) return;
        _worker = PuppetMgr.Instance.SpawnWorker(workerId, _buildingData.GetProp(BuildingProperty.MoveSpeed), _counter, _buildingData.Config.service);
        SetWorkIngredients(true);
    }
    public override void RefreshMoveSpeed()
    {
        _worker?.SetSpeed(_buildingData.GetProp(BuildingProperty.MoveSpeed));
    }
    private void StartDeliver()
    {
        if(_worker == null || _working) return;
        _deliverOrders.Clear();
        BuildingModule.Instance.DispatchOrder((int)_buildingData.GetProp(BuildingProperty.Capacity), ref _deliverOrders);
        if(_deliverOrders.Count > 0) {
            _working = true;
            foreach(var order in _deliverOrders) {
                _requires[IngredientType.Sausage] += order.sausage;
                _requires[IngredientType.Cheese] += order.cheese;
                _requires[IngredientType.Flour] += order.flour;
            }
            TakeIngredient();
        }
    }
    private void TakeIngredient()
    {
        if(_takeStep >= _takeIngredients.Count) {
            DeliverIngredient();
            return;
        }
        var curIngredient = _takeIngredients[_takeStep];
        if(_requires[curIngredient] == 0) {
            ++_takeStep;
            TakeIngredient();
        } else {
            _worker.MoveDo(_takeSpots[curIngredient], ActionDefine.Collect, 1f, OnTakeIngredient);
        }
    }
    private void OnTakeIngredient(PuppetCtrl puppet)
    {
        var curIngredient = _takeIngredients[_takeStep];
        _ingredients[curIngredient] = BuildingModule.Instance.TakeProduct(curIngredient, _requires[curIngredient]);
        SetWorkIngredients();
        ++_takeStep;
        TakeIngredient();
    }
    private void DeliverIngredient()
    {
        if((_ingredients[IngredientType.Sausage] == 0 && _ingredients[IngredientType.Cheese] == 0 && _ingredients[IngredientType.Flour] == 0) || _deliverStep >= _deliverOrders.Count) {
            BuildingModule.Instance.ReturnOrder(_deliverOrders);
            _worker.Move(_counter, Reset);
            return;
        }
        var curDeliverOrder = _deliverOrders[_deliverStep];
        _worker.MoveDo(ConfigModule.Instance.GetSpot(curDeliverOrder.buildingId, SpotDefine.Deliver, 1), ActionDefine.Collect, 1f, OnDeliverIngredient);
    }
    private void OnDeliverIngredient(PuppetCtrl puppet)
    {
        var curDeliverOrder = _deliverOrders[_deliverStep];
        BuildingModule.Instance.DeliverBuildingOrder(ref curDeliverOrder, ref _ingredients);
        SetWorkIngredients();
        ++_deliverStep;
        DeliverIngredient();
    }
    private void Reset(PuppetCtrl puppet)
    {
        _takeStep = 0;
        _deliverStep = 0;
        foreach(var deliveryOrder in _deliverOrders) {
            deliveryOrder.Return();
        }
        _deliverOrders.Clear();
        foreach(var key in _ingredientTypes) {
            _requires[key] = 0;
        }

        _working = false;
    }
    private void SetWorkIngredients(bool init = false)
    {
        _worker?.SetIngredients(_ingredients[IngredientType.Sausage], _ingredients[IngredientType.Cheese], _ingredients[IngredientType.Flour], init);
    }
    private void Update()
    {
        if(_timer < 1) {
            _timer += Time.deltaTime;
            return;
        }
        _timer = 0;
        StartDeliver();
    }
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Lemegeton;

public class StorageDivisionCtrl : BuildingCtrl
{
    private StorageHubProgressView _storageHubView;
    private string _product;
    public override void InitFlow()
    {
        _product = _buildingData.Config.product;
        EventModule.Instance.Register(EventDefine.StorageIngredient, OnStorageIngredientEvent);
    }
    void OnDestroy()
    {
        EventModule.Instance?.UnRegister(EventDefine.StorageIngredient, OnStorageIngredientEvent);
    }
    public override async Task LoadExtraHub() 
    {
        _storageHubView = await LoadModComponentTask<StorageHubProgressView>(ConfigModule.Instance.GetMod(GetStorageHubPath()), transform);
        _storageHubView?.SetCount(BuildingModule.Instance.GetProductCount(_product), (int)_buildingData.GetChildProps(BuildingProperty.Capacity), _buildingData.GetLevel());
    }
    private string GetStorageHubPath()
    {
        return string.Format("{0}_{1}_{2}_{3}", _buildingData.Config.id, "hub", _buildingData.Config.product, "progress");
    }
    private void OnStorageIngredientEvent(Component sender, EventArgs e)
    {
        StorageIngredientArgs args = e as StorageIngredientArgs;
        if(args.product == _product) {
            _storageHubView?.SetCount(BuildingModule.Instance.GetProductCount(_product), (int)_buildingData.GetChildProps(BuildingProperty.Capacity), _buildingData.GetLevel());
        }
    }
    public override void RefreshCapacity()
    {
        _storageHubView?.SetCount(BuildingModule.Instance.GetProductCount(_product), (int)_buildingData.GetChildProps(BuildingProperty.Capacity), _buildingData.GetLevel());
    }
}

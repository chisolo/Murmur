using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnhanceProperty
{
    public Dictionary<string, float> _valueEnhanceProps;
    public Dictionary<string, float> _ratioEnhanceProps;
    
    public EnhanceProperty(List<string> propTypes)
    {
        _valueEnhanceProps = new Dictionary<string, float>();
        _ratioEnhanceProps = new Dictionary<string, float>();
        foreach(var propType in propTypes) {
            _valueEnhanceProps.Add(propType, 0);
            _ratioEnhanceProps.Add(propType, 0);
        }
    }

    public void SetProp(string prop, float value, bool ratio)
    {
        if(ratio) _ratioEnhanceProps[prop] += value;
        else _valueEnhanceProps[prop] += value;
    }

    public void Clear()
    {
        var vKeys =  _valueEnhanceProps.Keys.ToArray<string>();
        foreach(var key in vKeys) {
            _valueEnhanceProps[key] = 0;
        }
        var rKeys =  _ratioEnhanceProps.Keys.ToArray<string>();
        foreach(var key in rKeys) {
            _ratioEnhanceProps[key] = 0;
        }
    }
    public float ValueEnhance(string prop)
    {
        return _valueEnhanceProps.TryGetValue(prop, out var value) ? value : 0;
    }

    public float RatioEnhance(string prop)
    {
        return _ratioEnhanceProps.TryGetValue(prop, out var value) ? value : 0;
    }
}

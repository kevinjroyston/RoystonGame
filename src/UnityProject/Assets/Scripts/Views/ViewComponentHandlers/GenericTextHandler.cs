﻿using Assets.Scripts.Networking.DataModels;
using Assets.Scripts.Views.DataModels;
using Assets.Scripts.Views.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TypeEnums;

public class GenericTextHandler : MonoBehaviour, HandlerInterface
{
    [Tooltip("If already applied to text object can be left blank")]
    public Text TextComponent;

    public List<HandlerId> HandlerIds => HandlerType.Strings.ToHandlerIdList(this.textType);
    public HandlerScope Scope => textType.GetScope();
    public StringType textType;

    public void UpdateValue(UnityField<string> field)
    {
         TextComponent.text = field?.Value ?? string.Empty;
    }

    public void UpdateValue(List<dynamic> objects)
    {
        UpdateValue(objects[0]);
    }
}
﻿using UnityEngine;
using System.Collections;
using SLua;

public class LuaBaseEvent : MonoBehaviour
{

    protected LuaFunction _luaFunc;
    protected string _messageName;

    public LuaFunction LuaCallback
    {
        set
        {
            _luaFunc = value;
        }
    }
}
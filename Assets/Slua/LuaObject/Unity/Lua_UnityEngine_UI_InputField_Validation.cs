﻿using UnityEngine;
using System;
using LuaInterface;
using SLua;
using System.Collections.Generic;
public class Lua_UnityEngine_UI_InputField_Validation : LuaObject {
	static public void reg(IntPtr l) {
		getEnumTable(l,"UnityEngine.UI.InputField.Validation");
		addMember(l,0,"None");
		addMember(l,1,"Integer");
		addMember(l,2,"Float");
		addMember(l,3,"Alphanumeric");
		addMember(l,4,"Username");
		addMember(l,5,"Name");
		LuaDLL.lua_pop(l, 1);
	}
}

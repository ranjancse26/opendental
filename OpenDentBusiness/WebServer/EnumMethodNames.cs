﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness{
	///<summary>MethodNames for retrieving datasets.</summary>
	public enum MethodNameDS{
		AccountModule_GetAll,
		AccountModule_GetPayPlanAmort,
		AccountModule_GetStatement,
		Appointment_GetApptEdit,
		Appointment_RefreshPeriod,
		Appointment_RefreshOneApt,
		Chart_GetAll,
		CovCats_RefreshCache
	}

	///<summary>MethodNames for retrieving tables.</summary>
	public enum MethodNameTable{
		Account_RefreshCache,
		AccountingAutoPay_RefreshCache,
		AppointmentRule_RefreshCache,
		ApptViewItem_RefreshCache,
		Definition_RefreshCache,
		GroupPermission_RefreshCache,
		MountDef_RefreshCache,
		Prefs_RefreshCache,
		Providers_RefreshCache,
		Userod_RefreshCache
	}
}

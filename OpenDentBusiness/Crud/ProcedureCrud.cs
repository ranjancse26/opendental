//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	internal class ProcedureCrud {
		///<summary>Gets one Procedure object from the database using the primary key.  Returns null if not found.</summary>
		internal static Procedure SelectOne(long procNum){
			string command="SELECT * FROM procedurelog "
				+"WHERE ProcNum = "+POut.Long(procNum);
			List<Procedure> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Procedure object from the database using a query.</summary>
		internal static Procedure SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Procedure> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Procedure object from the database using a query.</summary>
		internal static List<Procedure> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Procedure> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		internal static List<Procedure> TableToList(DataTable table){
			List<Procedure> retVal=new List<Procedure>();
			Procedure procedure;
			for(int i=0;i<table.Rows.Count;i++) {
				procedure=new Procedure();
				procedure.ProcNum           = PIn.Long  (table.Rows[i]["ProcNum"].ToString());
				procedure.PatNum            = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				procedure.AptNum            = PIn.Long  (table.Rows[i]["AptNum"].ToString());
				procedure.OldCode           = PIn.String(table.Rows[i]["OldCode"].ToString());
				procedure.ProcDate          = PIn.DateT (table.Rows[i]["ProcDate"].ToString());
				procedure.ProcFee           = PIn.Double(table.Rows[i]["ProcFee"].ToString());
				procedure.Surf              = PIn.String(table.Rows[i]["Surf"].ToString());
				procedure.ToothNum          = PIn.String(table.Rows[i]["ToothNum"].ToString());
				procedure.ToothRange        = PIn.String(table.Rows[i]["ToothRange"].ToString());
				procedure.Priority          = PIn.Long  (table.Rows[i]["Priority"].ToString());
				procedure.ProcStatus        = (ProcStat)PIn.Int(table.Rows[i]["ProcStatus"].ToString());
				procedure.ProvNum           = PIn.Long  (table.Rows[i]["ProvNum"].ToString());
				procedure.Dx                = PIn.Long  (table.Rows[i]["Dx"].ToString());
				procedure.PlannedAptNum     = PIn.Long  (table.Rows[i]["PlannedAptNum"].ToString());
				procedure.PlaceService      = (PlaceOfService)PIn.Int(table.Rows[i]["PlaceService"].ToString());
				procedure.Prosthesis        = PIn.String(table.Rows[i]["Prosthesis"].ToString());
				procedure.DateOriginalProsth= PIn.Date  (table.Rows[i]["DateOriginalProsth"].ToString());
				procedure.ClaimNote         = PIn.String(table.Rows[i]["ClaimNote"].ToString());
				procedure.DateEntryC        = PIn.Date  (table.Rows[i]["DateEntryC"].ToString());
				procedure.ClinicNum         = PIn.Long  (table.Rows[i]["ClinicNum"].ToString());
				procedure.MedicalCode       = PIn.String(table.Rows[i]["MedicalCode"].ToString());
				procedure.DiagnosticCode    = PIn.String(table.Rows[i]["DiagnosticCode"].ToString());
				procedure.IsPrincDiag       = PIn.Bool  (table.Rows[i]["IsPrincDiag"].ToString());
				procedure.ProcNumLab        = PIn.Long  (table.Rows[i]["ProcNumLab"].ToString());
				procedure.BillingTypeOne    = PIn.Long  (table.Rows[i]["BillingTypeOne"].ToString());
				procedure.BillingTypeTwo    = PIn.Long  (table.Rows[i]["BillingTypeTwo"].ToString());
				procedure.CodeNum           = PIn.Long  (table.Rows[i]["CodeNum"].ToString());
				procedure.CodeMod1          = PIn.String(table.Rows[i]["CodeMod1"].ToString());
				procedure.CodeMod2          = PIn.String(table.Rows[i]["CodeMod2"].ToString());
				procedure.CodeMod3          = PIn.String(table.Rows[i]["CodeMod3"].ToString());
				procedure.CodeMod4          = PIn.String(table.Rows[i]["CodeMod4"].ToString());
				procedure.RevCode           = PIn.String(table.Rows[i]["RevCode"].ToString());
				procedure.UnitCode          = PIn.String(table.Rows[i]["UnitCode"].ToString());
				procedure.UnitQty           = PIn.Int   (table.Rows[i]["UnitQty"].ToString());
				procedure.BaseUnits         = PIn.Int   (table.Rows[i]["BaseUnits"].ToString());
				procedure.StartTime         = PIn.Int   (table.Rows[i]["StartTime"].ToString());
				procedure.StopTime          = PIn.Int   (table.Rows[i]["StopTime"].ToString());
				procedure.DateTP            = PIn.Date  (table.Rows[i]["DateTP"].ToString());
				procedure.SiteNum           = PIn.Long  (table.Rows[i]["SiteNum"].ToString());
				procedure.HideGraphics      = PIn.Bool  (table.Rows[i]["HideGraphics"].ToString());
				retVal.Add(procedure);
			}
			return retVal;
		}

		///<summary>Inserts one Procedure into the database.  Returns the new priKey.</summary>
		internal static long Insert(Procedure procedure){
			return Insert(procedure,false);
		}

		///<summary>Inserts one Procedure into the database.  Provides option to use the existing priKey.</summary>
		internal static long Insert(Procedure procedure,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				procedure.ProcNum=ReplicationServers.GetKey("procedurelog","ProcNum");
			}
			string command="INSERT INTO procedurelog (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="ProcNum,";
			}
			command+="PatNum,AptNum,OldCode,ProcDate,ProcFee,Surf,ToothNum,ToothRange,Priority,ProcStatus,ProvNum,Dx,PlannedAptNum,PlaceService,Prosthesis,DateOriginalProsth,ClaimNote,DateEntryC,ClinicNum,MedicalCode,DiagnosticCode,IsPrincDiag,ProcNumLab,BillingTypeOne,BillingTypeTwo,CodeNum,CodeMod1,CodeMod2,CodeMod3,CodeMod4,RevCode,UnitCode,UnitQty,BaseUnits,StartTime,StopTime,DateTP,SiteNum,HideGraphics) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(procedure.ProcNum)+",";
			}
			command+=
				     POut.Long  (procedure.PatNum)+","
				+    POut.Long  (procedure.AptNum)+","
				+"'"+POut.String(procedure.OldCode)+"',"
				+    POut.DateT (procedure.ProcDate)+","
				+"'"+POut.Double(procedure.ProcFee)+"',"
				+"'"+POut.String(procedure.Surf)+"',"
				+"'"+POut.String(procedure.ToothNum)+"',"
				+"'"+POut.String(procedure.ToothRange)+"',"
				+    POut.Long  (procedure.Priority)+","
				+    POut.Int   ((int)procedure.ProcStatus)+","
				+    POut.Long  (procedure.ProvNum)+","
				+    POut.Long  (procedure.Dx)+","
				+    POut.Long  (procedure.PlannedAptNum)+","
				+    POut.Int   ((int)procedure.PlaceService)+","
				+"'"+POut.String(procedure.Prosthesis)+"',"
				+    POut.Date  (procedure.DateOriginalProsth)+","
				+"'"+POut.String(procedure.ClaimNote)+"',"
				+"NOW(),"
				+    POut.Long  (procedure.ClinicNum)+","
				+"'"+POut.String(procedure.MedicalCode)+"',"
				+"'"+POut.String(procedure.DiagnosticCode)+"',"
				+    POut.Bool  (procedure.IsPrincDiag)+","
				+    POut.Long  (procedure.ProcNumLab)+","
				+    POut.Long  (procedure.BillingTypeOne)+","
				+    POut.Long  (procedure.BillingTypeTwo)+","
				+    POut.Long  (procedure.CodeNum)+","
				+"'"+POut.String(procedure.CodeMod1)+"',"
				+"'"+POut.String(procedure.CodeMod2)+"',"
				+"'"+POut.String(procedure.CodeMod3)+"',"
				+"'"+POut.String(procedure.CodeMod4)+"',"
				+"'"+POut.String(procedure.RevCode)+"',"
				+"'"+POut.String(procedure.UnitCode)+"',"
				+    POut.Int   (procedure.UnitQty)+","
				+    POut.Int   (procedure.BaseUnits)+","
				+    POut.Int   (procedure.StartTime)+","
				+    POut.Int   (procedure.StopTime)+","
				+    POut.Date  (procedure.DateTP)+","
				+    POut.Long  (procedure.SiteNum)+","
				+    POut.Bool  (procedure.HideGraphics)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				procedure.ProcNum=Db.NonQ(command,true);
			}
			return procedure.ProcNum;
		}

		///<summary>Updates one Procedure in the database.</summary>
		internal static void Update(Procedure procedure){
			string command="UPDATE procedurelog SET "
				+"PatNum            =  "+POut.Long  (procedure.PatNum)+", "
				+"AptNum            =  "+POut.Long  (procedure.AptNum)+", "
				+"OldCode           = '"+POut.String(procedure.OldCode)+"', "
				+"ProcDate          =  "+POut.DateT (procedure.ProcDate)+", "
				+"ProcFee           = '"+POut.Double(procedure.ProcFee)+"', "
				+"Surf              = '"+POut.String(procedure.Surf)+"', "
				+"ToothNum          = '"+POut.String(procedure.ToothNum)+"', "
				+"ToothRange        = '"+POut.String(procedure.ToothRange)+"', "
				+"Priority          =  "+POut.Long  (procedure.Priority)+", "
				+"ProcStatus        =  "+POut.Int   ((int)procedure.ProcStatus)+", "
				+"ProvNum           =  "+POut.Long  (procedure.ProvNum)+", "
				+"Dx                =  "+POut.Long  (procedure.Dx)+", "
				+"PlannedAptNum     =  "+POut.Long  (procedure.PlannedAptNum)+", "
				+"PlaceService      =  "+POut.Int   ((int)procedure.PlaceService)+", "
				+"Prosthesis        = '"+POut.String(procedure.Prosthesis)+"', "
				+"DateOriginalProsth=  "+POut.Date  (procedure.DateOriginalProsth)+", "
				+"ClaimNote         = '"+POut.String(procedure.ClaimNote)+"', "
				+"DateEntryC        =  "+POut.Date  (procedure.DateEntryC)+", "
				+"ClinicNum         =  "+POut.Long  (procedure.ClinicNum)+", "
				+"MedicalCode       = '"+POut.String(procedure.MedicalCode)+"', "
				+"DiagnosticCode    = '"+POut.String(procedure.DiagnosticCode)+"', "
				+"IsPrincDiag       =  "+POut.Bool  (procedure.IsPrincDiag)+", "
				+"ProcNumLab        =  "+POut.Long  (procedure.ProcNumLab)+", "
				+"BillingTypeOne    =  "+POut.Long  (procedure.BillingTypeOne)+", "
				+"BillingTypeTwo    =  "+POut.Long  (procedure.BillingTypeTwo)+", "
				+"CodeNum           =  "+POut.Long  (procedure.CodeNum)+", "
				+"CodeMod1          = '"+POut.String(procedure.CodeMod1)+"', "
				+"CodeMod2          = '"+POut.String(procedure.CodeMod2)+"', "
				+"CodeMod3          = '"+POut.String(procedure.CodeMod3)+"', "
				+"CodeMod4          = '"+POut.String(procedure.CodeMod4)+"', "
				+"RevCode           = '"+POut.String(procedure.RevCode)+"', "
				+"UnitCode          = '"+POut.String(procedure.UnitCode)+"', "
				+"UnitQty           =  "+POut.Int   (procedure.UnitQty)+", "
				+"BaseUnits         =  "+POut.Int   (procedure.BaseUnits)+", "
				+"StartTime         =  "+POut.Int   (procedure.StartTime)+", "
				+"StopTime          =  "+POut.Int   (procedure.StopTime)+", "
				+"DateTP            =  "+POut.Date  (procedure.DateTP)+", "
				+"SiteNum           =  "+POut.Long  (procedure.SiteNum)+", "
				+"HideGraphics      =  "+POut.Bool  (procedure.HideGraphics)+" "
				+"WHERE ProcNum = "+POut.Long(procedure.ProcNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Procedure in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		internal static void Update(Procedure procedure,Procedure oldProcedure){
			string command="";
			if(procedure.PatNum != oldProcedure.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(procedure.PatNum)+"";
			}
			if(procedure.AptNum != oldProcedure.AptNum) {
				if(command!=""){ command+=",";}
				command+="AptNum = "+POut.Long(procedure.AptNum)+"";
			}
			if(procedure.OldCode != oldProcedure.OldCode) {
				if(command!=""){ command+=",";}
				command+="OldCode = '"+POut.String(procedure.OldCode)+"'";
			}
			if(procedure.ProcDate != oldProcedure.ProcDate) {
				if(command!=""){ command+=",";}
				command+="ProcDate = "+POut.DateT(procedure.ProcDate)+"";
			}
			if(procedure.ProcFee != oldProcedure.ProcFee) {
				if(command!=""){ command+=",";}
				command+="ProcFee = '"+POut.Double(procedure.ProcFee)+"'";
			}
			if(procedure.Surf != oldProcedure.Surf) {
				if(command!=""){ command+=",";}
				command+="Surf = '"+POut.String(procedure.Surf)+"'";
			}
			if(procedure.ToothNum != oldProcedure.ToothNum) {
				if(command!=""){ command+=",";}
				command+="ToothNum = '"+POut.String(procedure.ToothNum)+"'";
			}
			if(procedure.ToothRange != oldProcedure.ToothRange) {
				if(command!=""){ command+=",";}
				command+="ToothRange = '"+POut.String(procedure.ToothRange)+"'";
			}
			if(procedure.Priority != oldProcedure.Priority) {
				if(command!=""){ command+=",";}
				command+="Priority = "+POut.Long(procedure.Priority)+"";
			}
			if(procedure.ProcStatus != oldProcedure.ProcStatus) {
				if(command!=""){ command+=",";}
				command+="ProcStatus = "+POut.Int   ((int)procedure.ProcStatus)+"";
			}
			if(procedure.ProvNum != oldProcedure.ProvNum) {
				if(command!=""){ command+=",";}
				command+="ProvNum = "+POut.Long(procedure.ProvNum)+"";
			}
			if(procedure.Dx != oldProcedure.Dx) {
				if(command!=""){ command+=",";}
				command+="Dx = "+POut.Long(procedure.Dx)+"";
			}
			if(procedure.PlannedAptNum != oldProcedure.PlannedAptNum) {
				if(command!=""){ command+=",";}
				command+="PlannedAptNum = "+POut.Long(procedure.PlannedAptNum)+"";
			}
			if(procedure.PlaceService != oldProcedure.PlaceService) {
				if(command!=""){ command+=",";}
				command+="PlaceService = "+POut.Int   ((int)procedure.PlaceService)+"";
			}
			if(procedure.Prosthesis != oldProcedure.Prosthesis) {
				if(command!=""){ command+=",";}
				command+="Prosthesis = '"+POut.String(procedure.Prosthesis)+"'";
			}
			if(procedure.DateOriginalProsth != oldProcedure.DateOriginalProsth) {
				if(command!=""){ command+=",";}
				command+="DateOriginalProsth = "+POut.Date(procedure.DateOriginalProsth)+"";
			}
			if(procedure.ClaimNote != oldProcedure.ClaimNote) {
				if(command!=""){ command+=",";}
				command+="ClaimNote = '"+POut.String(procedure.ClaimNote)+"'";
			}
			if(procedure.DateEntryC != oldProcedure.DateEntryC) {
				if(command!=""){ command+=",";}
				command+="DateEntryC = "+POut.Date(procedure.DateEntryC)+"";
			}
			if(procedure.ClinicNum != oldProcedure.ClinicNum) {
				if(command!=""){ command+=",";}
				command+="ClinicNum = "+POut.Long(procedure.ClinicNum)+"";
			}
			if(procedure.MedicalCode != oldProcedure.MedicalCode) {
				if(command!=""){ command+=",";}
				command+="MedicalCode = '"+POut.String(procedure.MedicalCode)+"'";
			}
			if(procedure.DiagnosticCode != oldProcedure.DiagnosticCode) {
				if(command!=""){ command+=",";}
				command+="DiagnosticCode = '"+POut.String(procedure.DiagnosticCode)+"'";
			}
			if(procedure.IsPrincDiag != oldProcedure.IsPrincDiag) {
				if(command!=""){ command+=",";}
				command+="IsPrincDiag = "+POut.Bool(procedure.IsPrincDiag)+"";
			}
			if(procedure.ProcNumLab != oldProcedure.ProcNumLab) {
				if(command!=""){ command+=",";}
				command+="ProcNumLab = "+POut.Long(procedure.ProcNumLab)+"";
			}
			if(procedure.BillingTypeOne != oldProcedure.BillingTypeOne) {
				if(command!=""){ command+=",";}
				command+="BillingTypeOne = "+POut.Long(procedure.BillingTypeOne)+"";
			}
			if(procedure.BillingTypeTwo != oldProcedure.BillingTypeTwo) {
				if(command!=""){ command+=",";}
				command+="BillingTypeTwo = "+POut.Long(procedure.BillingTypeTwo)+"";
			}
			if(procedure.CodeNum != oldProcedure.CodeNum) {
				if(command!=""){ command+=",";}
				command+="CodeNum = "+POut.Long(procedure.CodeNum)+"";
			}
			if(procedure.CodeMod1 != oldProcedure.CodeMod1) {
				if(command!=""){ command+=",";}
				command+="CodeMod1 = '"+POut.String(procedure.CodeMod1)+"'";
			}
			if(procedure.CodeMod2 != oldProcedure.CodeMod2) {
				if(command!=""){ command+=",";}
				command+="CodeMod2 = '"+POut.String(procedure.CodeMod2)+"'";
			}
			if(procedure.CodeMod3 != oldProcedure.CodeMod3) {
				if(command!=""){ command+=",";}
				command+="CodeMod3 = '"+POut.String(procedure.CodeMod3)+"'";
			}
			if(procedure.CodeMod4 != oldProcedure.CodeMod4) {
				if(command!=""){ command+=",";}
				command+="CodeMod4 = '"+POut.String(procedure.CodeMod4)+"'";
			}
			if(procedure.RevCode != oldProcedure.RevCode) {
				if(command!=""){ command+=",";}
				command+="RevCode = '"+POut.String(procedure.RevCode)+"'";
			}
			if(procedure.UnitCode != oldProcedure.UnitCode) {
				if(command!=""){ command+=",";}
				command+="UnitCode = '"+POut.String(procedure.UnitCode)+"'";
			}
			if(procedure.UnitQty != oldProcedure.UnitQty) {
				if(command!=""){ command+=",";}
				command+="UnitQty = "+POut.Int(procedure.UnitQty)+"";
			}
			if(procedure.BaseUnits != oldProcedure.BaseUnits) {
				if(command!=""){ command+=",";}
				command+="BaseUnits = "+POut.Int(procedure.BaseUnits)+"";
			}
			if(procedure.StartTime != oldProcedure.StartTime) {
				if(command!=""){ command+=",";}
				command+="StartTime = "+POut.Int(procedure.StartTime)+"";
			}
			if(procedure.StopTime != oldProcedure.StopTime) {
				if(command!=""){ command+=",";}
				command+="StopTime = "+POut.Int(procedure.StopTime)+"";
			}
			if(procedure.DateTP != oldProcedure.DateTP) {
				if(command!=""){ command+=",";}
				command+="DateTP = "+POut.Date(procedure.DateTP)+"";
			}
			if(procedure.SiteNum != oldProcedure.SiteNum) {
				if(command!=""){ command+=",";}
				command+="SiteNum = "+POut.Long(procedure.SiteNum)+"";
			}
			if(procedure.HideGraphics != oldProcedure.HideGraphics) {
				if(command!=""){ command+=",";}
				command+="HideGraphics = "+POut.Bool(procedure.HideGraphics)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE procedurelog SET "+command
				+" WHERE ProcNum = "+POut.Long(procedure.ProcNum);
			Db.NonQ(command);
		}

		//Delete not allowed for this table
		//internal static void Delete(long procNum){
		//
		//}

	}
}
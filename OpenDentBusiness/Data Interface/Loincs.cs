using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Loincs{
		#region CachePattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add RefreshCache and FillCache 
		//to the Cache.cs file with all the other Cache types.

		///<summary>A list of all Loincs.</summary>
		private static List<Loinc> listt;

		///<summary>A list of all Loincs.</summary>
		public static List<Loinc> Listt{
			get {
				if(listt==null) {
					RefreshCache();
				}
				return listt;
			}
			set {
				listt=value;
			}
		}

		///<summary></summary>
		public static DataTable RefreshCache(){
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			string command="SELECT * FROM loinc ORDER BY ItemOrder";//stub query probably needs to be changed
			DataTable table=Cache.GetTableRemotelyIfNeeded(MethodBase.GetCurrentMethod(),command);
			table.TableName="Loinc";
			FillCache(table);
			return table;
		}

		///<summary></summary>
		public static void FillCache(DataTable table){
			//No need to check RemotingRole; no call to db.
			listt=Crud.LoincCrud.TableToList(table);
		}
		#endregion

		///<summary></summary>
		public static long Insert(Loinc lOINC){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				lOINC.LoincNum=Meth.GetLong(MethodBase.GetCurrentMethod(),lOINC);
				return lOINC.LoincNum;
			}
			return Crud.LoincCrud.Insert(lOINC);
		}

		///<summary></summary>
		public static List<Loinc> GetBySearchString(string searchText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod(),searchText);
			}
			string command="SELECT * FROM loinc WHERE LoincCode LIKE '%"+POut.String(searchText)+"%' OR NameLongCommon LIKE '%"+POut.String(searchText)+"%'";
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary>Gets one Loinc from the db based on LoincCode, returns null if not found.</summary>
		public static Loinc GetByCode(string lOINCCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Loinc>(MethodBase.GetCurrentMethod(),lOINCCode);
			}
			string command="SELECT * FROM loinc WHERE LoincCode='"+POut.String(lOINCCode)+"'";
			List<Loinc> retVal=Crud.LoincCrud.SelectMany(command);
			if(retVal.Count>0) {
				return retVal[0];
			}
			return null;
		}

		///<summary>CAUTION, this empties the entire loinc table. "DELETE FROM loinc"</summary>
		public static void DeleteAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="DELETE FROM loinc";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE loinc AUTO_INCREMENT = 1";//resets the primary key to start counting from 1 again.
				Db.NonQ(command);
			}
			return;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<Loinc> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Loinc>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM loinc WHERE PatNum = "+POut.Long(patNum);
			return Crud.LoincCrud.SelectMany(command);
		}

		///<summary>Gets one Loinc from the db.</summary>
		public static Loinc GetOne(long lOINCNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Loinc>(MethodBase.GetCurrentMethod(),lOINCNum);
			}
			return Crud.LoincCrud.SelectOne(lOINCNum);
		}

		///<summary></summary>
		public static void Update(Loinc lOINC){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),lOINC);
				return;
			}
			Crud.LoincCrud.Update(lOINC);
		}

		///<summary></summary>
		public static void Delete(long lOINCNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),lOINCNum);
				return;
			}
			string command= "DELETE FROM loinc WHERE LoincNum = "+POut.Long(lOINCNum);
			Db.NonQ(command);
		}
		*/



	}
}
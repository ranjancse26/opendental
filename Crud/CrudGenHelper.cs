﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace Crud {
	public class CrudGenHelper {
		///<summary>Will throw exception if no primary key attribute defined.</summary>
		public static FieldInfo GetPriKey(FieldInfo[] fields,string tableName){
			for(int i=0;i<fields.Length;i++) {
				object[] attributes = fields[i].GetCustomAttributes(typeof(CrudColumnAttribute),true);
				if(attributes.Length!=1) {
					continue;
				}
				if(((CrudColumnAttribute)attributes[0]).IsPriKey) {
					return fields[i];
				}
			}
			throw new ApplicationException("No primary key defined for "+tableName);
		}

		///<summary>The name of the table in the database.  By default, the lowercase name of the class type.</summary>
		public static string GetTableName(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return typeClass.Name.ToLower();
			}
			return((CrudTableAttribute)attributes[0]).TableName;
		}

		///<summary></summary>
		public static bool IsDeleteForbidden(Type typeClass) {
			object[] attributes = typeClass.GetCustomAttributes(typeof(CrudTableAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			return ((CrudTableAttribute)attributes[0]).IsDeleteForbidden;
		}

		///<summary>This also excludes fields that are not in the database, like patient.Age.</summary>
		public static List<FieldInfo> GetFieldsExceptPriKey(FieldInfo[] fields,FieldInfo priKey) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(fields[i].Name==priKey.Name) {
					continue;
				}
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>This only excludes fields that are not in the database, like patient.Age.</summary>
		public static List<FieldInfo> GetFieldsExceptNotDb(FieldInfo[] fields) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				retVal.Add(fields[i]);
			}
			return retVal;
		}

		///<summary>This gets all new fields which are found in the table definition but not in the database.  Result will be empty if the table itself is not in the database.</summary>
		public static List<FieldInfo> GetNewFields(FieldInfo[] fields,Type typeClass,string dbName) {
			string tablename=GetTableName(typeClass);
			string command="SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '"+dbName+"' AND table_name = '"+tablename+"'";
			if(DataCore.GetScalar(command)!="1") {
				return new List<FieldInfo>();
			}
			command="SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS "
				+"WHERE table_name = '"+tablename+"' AND table_schema = '"+dbName+"'";
			DataTable table=DataCore.GetTable(command);
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fields.Length;i++) {
				if(IsNotDbColumn(fields[i])) {
					continue;
				}
				bool found=false; ;
				for(int t=0;t<table.Rows.Count;t++) {
					if(table.Rows[t]["COLUMN_NAME"].ToString().ToLower()==fields[i].Name.ToLower()) {
						found=true;
					}
				}
				if(!found) {
					retVal.Add(fields[i]);
				}
			}
			return retVal;
		}

		///<summary>Pass in fields processed by GetFieldsExceptPriKey.  This quick method returns the bigint fields so that indexes can possibly be added.</summary>
		public static List<FieldInfo> GetBigIntFields(List<FieldInfo> fieldsExceptPri) {
			List<FieldInfo> retVal=new List<FieldInfo>();
			for(int i=0;i<fieldsExceptPri.Count;i++) {
				if(fieldsExceptPri[i].FieldType.Name=="Int64") {
					retVal.Add(fieldsExceptPri[i]);
				}
			}
			return retVal;
		}

		public static EnumCrudSpecialColType GetSpecialType(FieldInfo field) {
			object[] attributes = field.GetCustomAttributes(typeof(CrudColumnAttribute),true);
			if(attributes.Length==0) {
				return EnumCrudSpecialColType.None;
			}
			return ((CrudColumnAttribute)attributes[0]).SpecialType;
		}

		///<summary>Normally false</summary>
		public static bool IsNotDbColumn(FieldInfo field) {
			object[] attributes = field.GetCustomAttributes(typeof(CrudColumnAttribute),true);
			if(attributes.Length==0) {
				return false;
			}
			return ((CrudColumnAttribute)attributes[0]).IsNotDbColumn;
		}

		public static void ConnectToDatabase(string dbName){
			OpenDentBusiness.DataConnection dcon=new OpenDentBusiness.DataConnection();
			dcon.SetDb("localhost",dbName,"root","","","",DatabaseType.MySql);
			RemotingClient.RemotingRole=RemotingRole.ClientDirect;
		}

		///<summary>Makes sure the tablename is valid.  Goes through each column and makes sure that the column is present and that the type in the database is a supported type for this C# data type.  Throws exception if it fails.</summary>
		public static void ValidateTypes(Type typeClass,string dbName){
			string tablename=GetTableName(typeClass);
			string command="SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '"+dbName+"' AND table_name = '"+tablename+"'";
			if(DataCore.GetScalar(command)!="1"){
				return;//can't validate
			}
			command="SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS "
				+"WHERE table_name = '"+tablename+"' AND table_schema = '"+dbName+"'";
			DataTable table=DataCore.GetTable(command);
			FieldInfo[] fields=typeClass.GetFields();
			for(int i=0;i<fields.Length;i++){
				if(IsNotDbColumn(fields[i])){
					continue;
				}
				ValidateColumn(dbName,tablename,fields[i],table);
			}
		}

		public static void ValidateColumn(string dbName,string tablename,FieldInfo field,DataTable table){
			//make sure the column exists
			string dataTypeInDb="";
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i]["COLUMN_NAME"].ToString().ToLower()==field.Name.ToLower()){
					dataTypeInDb=table.Rows[i]["DATA_TYPE"].ToString();
				}
			}
			if(dataTypeInDb==""){
				return;//can't validate
			}
			EnumCrudSpecialColType specialColType=GetSpecialType(field);
			string dataTypeExpected="";
			string dataTypeExpected2="";//if an alternate datatype is allowed
			string dataTypeExpected3="";
			if(specialColType==EnumCrudSpecialColType.TimeStamp) {
				dataTypeExpected="timestamp";
			}
			else if(specialColType==EnumCrudSpecialColType.DateEntry) {
				dataTypeExpected="date";
			}
			else if(specialColType==EnumCrudSpecialColType.DateEntryEditable) {
				dataTypeExpected="date";
			}
			else if(specialColType==EnumCrudSpecialColType.DateT) {
				dataTypeExpected="datetime";
			}
			else if(specialColType==EnumCrudSpecialColType.DateTEntry) {
				dataTypeExpected="datetime";
			}
			else if(specialColType==EnumCrudSpecialColType.DateTEntryEditable) {
				dataTypeExpected="datetime";
			}
			else if(field.FieldType.IsEnum) {
				dataTypeExpected="tinyint";
				dataTypeExpected2="int";
			}
			else switch(field.FieldType.Name) {
				default:
					throw new ApplicationException("Type not yet supported: "+field.FieldType.Name);
				case "Boolean":
					dataTypeExpected="tinyint";
					break;
				case "Byte":
					dataTypeExpected="tinyint";
					break;
				case "Color":
					dataTypeExpected="int";
					break;
				case "DateTime"://Need to handle DateT fields here better.
					dataTypeExpected="date";
					break;
				case "Double":
					dataTypeExpected="double";
					break;
				case "Interval":
					dataTypeExpected="int";
					break;
				case "Int64":
					dataTypeExpected="bigint";
					break;
				case "Int32":
					dataTypeExpected="int";
					dataTypeExpected2="smallint";//ok as long as the coding is careful.  Less than ideal.
					//tinyint not allowed.  Change C# type to byte.
					break;
				case "Single":
					dataTypeExpected="float";
					break;
				case "String":
					dataTypeExpected="varchar";
					dataTypeExpected2="text";
					dataTypeExpected3="char";
					break;
				case "TimeSpan":
					dataTypeExpected="time";
					break;
			}
			if(dataTypeInDb!=dataTypeExpected && dataTypeInDb!=dataTypeExpected2 && dataTypeInDb!=dataTypeExpected3){
				throw new Exception(tablename+"."+field.Name+" type mismatch for type "+field.FieldType.Name+".  Found "+dataTypeInDb+", but expecting "+dataTypeExpected);
			}
		}





	}
}

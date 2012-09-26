//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace OpenDentBusiness.Crud{
	internal class StatementCrud {
		///<summary>Gets one Statement object from the database using the primary key.  Returns null if not found.</summary>
		internal static Statement SelectOne(long statementNum){
			string command="SELECT * FROM statement "
				+"WHERE StatementNum = "+POut.Long(statementNum);
			List<Statement> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Statement object from the database using a query.</summary>
		internal static Statement SelectOne(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Statement> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Statement objects from the database using a query.</summary>
		internal static List<Statement> SelectMany(string command){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Statement> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		internal static List<Statement> TableToList(DataTable table){
			List<Statement> retVal=new List<Statement>();
			Statement statement;
			for(int i=0;i<table.Rows.Count;i++) {
				statement=new Statement();
				statement.StatementNum = PIn.Long  (table.Rows[i]["StatementNum"].ToString());
				statement.PatNum       = PIn.Long  (table.Rows[i]["PatNum"].ToString());
				statement.DateSent     = PIn.Date  (table.Rows[i]["DateSent"].ToString());
				statement.DateRangeFrom= PIn.Date  (table.Rows[i]["DateRangeFrom"].ToString());
				statement.DateRangeTo  = PIn.Date  (table.Rows[i]["DateRangeTo"].ToString());
				statement.Note         = PIn.String(table.Rows[i]["Note"].ToString());
				statement.NoteBold     = PIn.String(table.Rows[i]["NoteBold"].ToString());
				statement.Mode_        = (StatementMode)PIn.Int(table.Rows[i]["Mode_"].ToString());
				statement.HidePayment  = PIn.Bool  (table.Rows[i]["HidePayment"].ToString());
				statement.SinglePatient= PIn.Bool  (table.Rows[i]["SinglePatient"].ToString());
				statement.Intermingled = PIn.Bool  (table.Rows[i]["Intermingled"].ToString());
				statement.IsSent       = PIn.Bool  (table.Rows[i]["IsSent"].ToString());
				statement.DocNum       = PIn.Long  (table.Rows[i]["DocNum"].ToString());
				statement.DateTStamp   = PIn.DateT (table.Rows[i]["DateTStamp"].ToString());
				statement.IsReceipt    = PIn.Bool  (table.Rows[i]["IsReceipt"].ToString());
				statement.IsInvoice    = PIn.Bool  (table.Rows[i]["IsInvoice"].ToString());
				statement.IsInvoiceCopy= PIn.Bool  (table.Rows[i]["IsInvoiceCopy"].ToString());
				retVal.Add(statement);
			}
			return retVal;
		}

		///<summary>Inserts one Statement into the database.  Returns the new priKey.</summary>
		internal static long Insert(Statement statement){
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				statement.StatementNum=DbHelper.GetNextOracleKey("statement","StatementNum");
				int loopcount=0;
				while(loopcount<100){
					try {
						return Insert(statement,true);
					}
					catch(Oracle.DataAccess.Client.OracleException ex){
						if(ex.Number==1 && ex.Message.ToLower().Contains("unique constraint") && ex.Message.ToLower().Contains("violated")){
							statement.StatementNum++;
							loopcount++;
						}
						else{
							throw ex;
						}
					}
				}
				throw new ApplicationException("Insert failed.  Could not generate primary key.");
			}
			else {
				return Insert(statement,false);
			}
		}

		///<summary>Inserts one Statement into the database.  Provides option to use the existing priKey.</summary>
		internal static long Insert(Statement statement,bool useExistingPK){
			if(!useExistingPK && PrefC.RandomKeys) {
				statement.StatementNum=ReplicationServers.GetKey("statement","StatementNum");
			}
			string command="INSERT INTO statement (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="StatementNum,";
			}
			command+="PatNum,DateSent,DateRangeFrom,DateRangeTo,Note,NoteBold,Mode_,HidePayment,SinglePatient,Intermingled,IsSent,DocNum,IsReceipt,IsInvoice,IsInvoiceCopy) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(statement.StatementNum)+",";
			}
			command+=
				     POut.Long  (statement.PatNum)+","
				+    POut.Date  (statement.DateSent)+","
				+    POut.Date  (statement.DateRangeFrom)+","
				+    POut.Date  (statement.DateRangeTo)+","
				+"'"+POut.String(statement.Note)+"',"
				+"'"+POut.String(statement.NoteBold)+"',"
				+    POut.Int   ((int)statement.Mode_)+","
				+    POut.Bool  (statement.HidePayment)+","
				+    POut.Bool  (statement.SinglePatient)+","
				+    POut.Bool  (statement.Intermingled)+","
				+    POut.Bool  (statement.IsSent)+","
				+    POut.Long  (statement.DocNum)+","
				//DateTStamp can only be set by MySQL
				+    POut.Bool  (statement.IsReceipt)+","
				+    POut.Bool  (statement.IsInvoice)+","
				+    POut.Bool  (statement.IsInvoiceCopy)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				statement.StatementNum=Db.NonQ(command,true);
			}
			return statement.StatementNum;
		}

		///<summary>Updates one Statement in the database.</summary>
		internal static void Update(Statement statement){
			string command="UPDATE statement SET "
				+"PatNum       =  "+POut.Long  (statement.PatNum)+", "
				+"DateSent     =  "+POut.Date  (statement.DateSent)+", "
				+"DateRangeFrom=  "+POut.Date  (statement.DateRangeFrom)+", "
				+"DateRangeTo  =  "+POut.Date  (statement.DateRangeTo)+", "
				+"Note         = '"+POut.String(statement.Note)+"', "
				+"NoteBold     = '"+POut.String(statement.NoteBold)+"', "
				+"Mode_        =  "+POut.Int   ((int)statement.Mode_)+", "
				+"HidePayment  =  "+POut.Bool  (statement.HidePayment)+", "
				+"SinglePatient=  "+POut.Bool  (statement.SinglePatient)+", "
				+"Intermingled =  "+POut.Bool  (statement.Intermingled)+", "
				+"IsSent       =  "+POut.Bool  (statement.IsSent)+", "
				+"DocNum       =  "+POut.Long  (statement.DocNum)+", "
				//DateTStamp can only be set by MySQL
				+"IsReceipt    =  "+POut.Bool  (statement.IsReceipt)+", "
				+"IsInvoice    =  "+POut.Bool  (statement.IsInvoice)+", "
				+"IsInvoiceCopy=  "+POut.Bool  (statement.IsInvoiceCopy)+" "
				+"WHERE StatementNum = "+POut.Long(statement.StatementNum);
			Db.NonQ(command);
		}

		///<summary>Updates one Statement in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.</summary>
		internal static void Update(Statement statement,Statement oldStatement){
			string command="";
			if(statement.PatNum != oldStatement.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(statement.PatNum)+"";
			}
			if(statement.DateSent != oldStatement.DateSent) {
				if(command!=""){ command+=",";}
				command+="DateSent = "+POut.Date(statement.DateSent)+"";
			}
			if(statement.DateRangeFrom != oldStatement.DateRangeFrom) {
				if(command!=""){ command+=",";}
				command+="DateRangeFrom = "+POut.Date(statement.DateRangeFrom)+"";
			}
			if(statement.DateRangeTo != oldStatement.DateRangeTo) {
				if(command!=""){ command+=",";}
				command+="DateRangeTo = "+POut.Date(statement.DateRangeTo)+"";
			}
			if(statement.Note != oldStatement.Note) {
				if(command!=""){ command+=",";}
				command+="Note = '"+POut.String(statement.Note)+"'";
			}
			if(statement.NoteBold != oldStatement.NoteBold) {
				if(command!=""){ command+=",";}
				command+="NoteBold = '"+POut.String(statement.NoteBold)+"'";
			}
			if(statement.Mode_ != oldStatement.Mode_) {
				if(command!=""){ command+=",";}
				command+="Mode_ = "+POut.Int   ((int)statement.Mode_)+"";
			}
			if(statement.HidePayment != oldStatement.HidePayment) {
				if(command!=""){ command+=",";}
				command+="HidePayment = "+POut.Bool(statement.HidePayment)+"";
			}
			if(statement.SinglePatient != oldStatement.SinglePatient) {
				if(command!=""){ command+=",";}
				command+="SinglePatient = "+POut.Bool(statement.SinglePatient)+"";
			}
			if(statement.Intermingled != oldStatement.Intermingled) {
				if(command!=""){ command+=",";}
				command+="Intermingled = "+POut.Bool(statement.Intermingled)+"";
			}
			if(statement.IsSent != oldStatement.IsSent) {
				if(command!=""){ command+=",";}
				command+="IsSent = "+POut.Bool(statement.IsSent)+"";
			}
			if(statement.DocNum != oldStatement.DocNum) {
				if(command!=""){ command+=",";}
				command+="DocNum = "+POut.Long(statement.DocNum)+"";
			}
			//DateTStamp can only be set by MySQL
			if(statement.IsReceipt != oldStatement.IsReceipt) {
				if(command!=""){ command+=",";}
				command+="IsReceipt = "+POut.Bool(statement.IsReceipt)+"";
			}
			if(statement.IsInvoice != oldStatement.IsInvoice) {
				if(command!=""){ command+=",";}
				command+="IsInvoice = "+POut.Bool(statement.IsInvoice)+"";
			}
			if(statement.IsInvoiceCopy != oldStatement.IsInvoiceCopy) {
				if(command!=""){ command+=",";}
				command+="IsInvoiceCopy = "+POut.Bool(statement.IsInvoiceCopy)+"";
			}
			if(command==""){
				return;
			}
			command="UPDATE statement SET "+command
				+" WHERE StatementNum = "+POut.Long(statement.StatementNum);
			Db.NonQ(command);
		}

		///<summary>Deletes one Statement from the database.</summary>
		internal static void Delete(long statementNum){
			string command="DELETE FROM statement "
				+"WHERE StatementNum = "+POut.Long(statementNum);
			Db.NonQ(command);
		}

	}
}
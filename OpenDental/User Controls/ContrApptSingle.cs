/*=============================================================================================================
Open Dental GPL license Copyright (C) 2003  Jordan Sparks, DMD.  http://www.open-dent.com,  www.docsparks.com
See header in FormOpenDental.cs for complete text.  Redistributions must retain this text.
===============================================================================================================*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{

	///<summary></summary>
	public class ContrApptSingle : System.Windows.Forms.UserControl{
		private System.ComponentModel.Container components = null;// Required designer variable.
		///<summary>Set on mouse down or from Appt module</summary>
		public static long ClickedAptNum;
		/// <summary>This is not the best place for this, but changing it now would cause bugs.  Set manually</summary>
		public static long SelectedAptNum;
		///<summary>True if this control is on the pinboard</summary>
		public bool ThisIsPinBoard;
		///<summary>Stores the shading info for the provider bars on the left of the appointments module</summary>
		public static int[][] ProvBar;
		///<summary>Stores the background bitmap for this control</summary>
		public Bitmap Shadow;
		private Font baseFont=new Font("Arial",8);
		private Font boldFont=new Font("Arial",8,FontStyle.Bold);
		///<summary>The actual slashes and Xs showing for the current view.</summary>
		private string patternShowing;
		///<summary>This is a datarow that stores most of the info necessary to draw appt.  It comes from the table obtained in Appointments.GetPeriodApptsTable().</summary>
		public DataRow DataRoww;
		///<summary>This table contains all appointment fields for all appointments in the period. It's obtained in Appointments.GetApptFields().</summary>
		public DataTable TableApptFields;
		///<summary>Indicator that account has procedures with no ins claim</summary>
		public bool FamHasInsNotSent;
		///<Summary>Will show the highlight around the edges.  For now, this is only used for pinboard.  The ordinary selected appt is set with SelectedAptNum.</Summary>
		public bool IsSelected;


		///<summary></summary>
		public ContrApptSingle(){
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			//Info=new InfoApt();
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
				if(Shadow!=null) {
					Shadow.Dispose();
				}
				if(baseFont!=null) {
					baseFont.Dispose();
				}
				if(boldFont!=null) {
					boldFont.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private void InitializeComponent(){
			// 
			// ContrApptSingle
			// 
			this.Name = "ContrApptSingle";
			this.Size = new System.Drawing.Size(85, 72);
			this.Load += new System.EventHandler(this.ContrApptSingle_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ContrApptSingle_MouseDown);

		}
		#endregion
		
		///<summary></summary>
		protected override void OnPaint(PaintEventArgs pea){
			//Graphics grfx=pea.Graphics;
			//grfx.DrawImage(shadow,0,0);
		}

		
		///<summary>This is only called when viewing appointments on the Appt module.  For Planned apt and pinboard, use SetSize instead so that the location won't change.</summary>
		public void SetLocation(){
			if(ContrApptSheet.IsWeeklyView) {
				Width=(int)ContrApptSheet.ColAptWidth;
				Location=new Point(ConvertToX(),ConvertToY());
			}
			else{
				Location=new Point(ConvertToX()+2,ConvertToY());
				Width=ContrApptSheet.ColWidth-5;
			}
			SetSize();
		}

		///<summary>Used from SetLocation. Also used for Planned apt and pinboard instead of SetLocation so that the location won't be altered.</summary>
		public void SetSize(){
			patternShowing=GetPatternShowing(DataRoww["Pattern"].ToString());
			//height is based on original 5 minute pattern. Might result in half-rows
			Height=DataRoww["Pattern"].ToString().Length*ContrApptSheet.Lh*ContrApptSheet.RowsPerIncr;
			//if(ContrApptSheet.TwoRowsPerIncrement){
			//	Height=Height*2;
			//}
			if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==10){
				Height=Height/2;
			}
			if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==15){
				Height=Height/3;
			}
			//if(ThisIsPinBoard){
				//hack:  it's at 99x96 right now
				//if(Height > ContrAppt.PinboardSize.Height-4)
				//	Height=ContrAppt.PinboardSize.Height-4;
				//if(Width > 99-2){
			//	Width=99-2;
				//}
			//}
		}
		
		///<summary>Called from SetLocation to establish X position of control.</summary>
		private int ConvertToX(){
			if(ContrApptSheet.IsWeeklyView) {
				//the next few lines are because we start on Monday instead of Sunday
				int dayofweek=(int)PIn.DateT(DataRoww["AptDateTime"].ToString()).DayOfWeek-1;
				if(dayofweek==-1) {
					dayofweek=6;
				}
				return ContrApptSheet.TimeWidth
					+ContrApptSheet.ColDayWidth*(dayofweek)+1
					+(int)(ContrApptSheet.ColAptWidth*(float)ApptViewItemL.GetIndexOp(PIn.Long(DataRoww["Op"].ToString())));
			}
			else {
				return ContrApptSheet.TimeWidth+ContrApptSheet.ProvWidth*ContrApptSheet.ProvCount
					+ContrApptSheet.ColWidth*(ApptViewItemL.GetIndexOp(PIn.Long(DataRoww["Op"].ToString())))+1;
					//Info.MyApt.Op))+1;
			}
		}

		///<summary>Called from SetLocation to establish Y position of control.  Also called from ContrAppt.RefreshDay when determining provBar markings. Does not round to the nearest row.</summary>
		public int ConvertToY(){
			DateTime aptDateTime=PIn.DateT(DataRoww["AptDateTime"].ToString());
			int retVal=(int)(((double)aptDateTime.Hour*(double)60
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
				+(double)aptDateTime.Minute
				/(double)PrefC.GetLong(PrefName.AppointmentTimeIncrement)
				)*(double)ContrApptSheet.Lh*ContrApptSheet.RowsPerIncr);
			return retVal;
		}

		///<summary>This converts the dbPattern in 5 minute interval into the pattern that will be viewed based on RowsPerIncrement and AppointmentTimeIncrement.  So it will always depend on the current view.Therefore, it should only be used for visual display purposes rather than within the FormAptEdit. If height of appointment allows a half row, then this includes an increment for that half row.</summary>
		public static string GetPatternShowing(string dbPattern){
			StringBuilder strBTime=new StringBuilder();
			for(int i=0;i<dbPattern.Length;i++){
				for(int j=0;j<ContrApptSheet.RowsPerIncr;j++){
					strBTime.Append(dbPattern.Substring(i,1));
				}
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==10) {
					i++;//skip
				}
				if(PrefC.GetLong(PrefName.AppointmentTimeIncrement)==15){
					i++;
					i++;//skip two
				}
			}
			return strBTime.ToString();
		}

		///<summary>It is planned to move some of this logic to OnPaint and use a true double buffer.</summary>
		public void CreateShadow(){
			if(this.Parent is ContrApptSheet) {
				bool isVisible=false;
				for(int j=0;j<ApptViewItemL.VisOps.Count;j++) {
					if(this.DataRoww["Op"].ToString()==OperatoryC.ListShort[ApptViewItemL.VisOps[j]].OperatoryNum.ToString()){
						isVisible=true;
					}
				}
				if(!isVisible){
					return;
				}
			}
			if(Shadow!=null) {
				Shadow.Dispose();
				Shadow=null;
			}
			if(Width<4){
				return;
			}
			if(Height<4){
				return;
			}
			Shadow=new Bitmap(Width,Height);
			Graphics g=Graphics.FromImage(Shadow);
			Pen penB=new Pen(Color.Black);
			Pen penW=new Pen(Color.White);
			Pen penGr=new Pen(Color.SlateGray);
			Pen penDG=new Pen(Color.DarkSlateGray);
			Pen penO;//provider outline color
			Color backColor;
			Color provColor;
			if(DataRoww["ProvNum"].ToString()!="0" && DataRoww["IsHygiene"].ToString()=="0"){//dentist
				provColor=Providers.GetColor(PIn.Long(DataRoww["ProvNum"].ToString()));
				penO=new Pen(Providers.GetOutlineColor(PIn.Long(DataRoww["ProvNum"].ToString())));
			}
			else if(DataRoww["ProvHyg"].ToString()!="0" && DataRoww["IsHygiene"].ToString()=="1"){//hygienist
				provColor=Providers.GetColor(PIn.Long(DataRoww["ProvHyg"].ToString()));
				penO=new Pen(Providers.GetOutlineColor(PIn.Long(DataRoww["ProvHyg"].ToString())));
			}
			else{//unknown
				provColor=Color.White;
				penO=new Pen(Color.Black);
			}
			if(PIn.Long(DataRoww["AptStatus"].ToString())==(int)ApptStatus.Complete){
				backColor=DefC.Long[(int)DefCat.AppointmentColors][3].ItemColor;
			}
			else if (PIn.Long(DataRoww["AptStatus"].ToString())==(int)ApptStatus.PtNote) {
				backColor=DefC.Long[(int)DefCat.AppointmentColors][7].ItemColor;
			}
			else if (PIn.Long(DataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNoteCompleted) {
				backColor = DefC.Long[(int)DefCat.AppointmentColors][10].ItemColor;
			}
			else {
				backColor=provColor;
				//We might want to do something interesting here.
			}
			SolidBrush backBrush=new SolidBrush(backColor);
			g.FillRectangle(backBrush,7,0,Width-7,Height);
			g.FillRectangle(Brushes.White,0,0,7,Height);
			g.DrawLine(penB,7,0,7,Height);
			Pen penTimediv=Pens.Silver;
			//g.TextRenderingHint=TextRenderingHint.SingleBitPerPixelGridFit;//to make printing clearer
			for(int i=0;i<patternShowing.Length;i++){//Info.MyApt.Pattern.Length;i++){
				if(patternShowing.Substring(i,1)=="X"){	
					g.FillRectangle(new SolidBrush(provColor),1,i*ContrApptSheet.Lh+1,6,ContrApptSheet.Lh);
				}
				else{
					//leave empty
				}
				if(Math.IEEERemainder((double)i,(double)ContrApptSheet.RowsPerIncr)==0){//0/1
					g.DrawLine(penTimediv,1,i*ContrApptSheet.Lh,6,i*ContrApptSheet.Lh);
				}	
			}
			//Highlighting border
			if(IsSelected	|| (!ThisIsPinBoard && DataRoww["AptNum"].ToString()==SelectedAptNum.ToString())) {
				//Left
				g.DrawLine(penO,8,1,8,Height-2);
				g.DrawLine(penO,9,1,9,Height-3);
				//Right
				g.DrawLine(penO,Width-2,1,Width-2,Height-2);
				g.DrawLine(penO,Width-3,2,Width-3,Height-3);
				//Top
				g.DrawLine(penO,8,1,Width-2,1);
				g.DrawLine(penO,8,2,Width-3,2);
				//bottom
				g.DrawLine(penO,9,Height-2,Width-2,Height-2);
				g.DrawLine(penO,10,Height-3,Width-3,Height-3);
			}
			//Draw all the main rows
			Point drawLoc=new Point(9,0);
			int elementI=0;
			while(drawLoc.Y<Height && elementI<ApptViewItemL.ApptRows.Count) {
				if(ApptViewItemL.ApptRows[elementI].ElementAlignment!=ApptViewAlignment.Main) {
					elementI++;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,ApptViewStackBehavior.Vertical,ApptViewAlignment.Main,backBrush);//set the drawLoc to a new point, based on space used by element
				elementI++;
			}
			//UR
			drawLoc=new Point(Width-1,0);//in the UR area, we refer to the upper right corner of each element.
			elementI=0;
			while(drawLoc.Y<Height && elementI<ApptViewItemL.ApptRows.Count) {
				if(ApptViewItemL.ApptRows[elementI].ElementAlignment!=ApptViewAlignment.UR) {
					elementI++;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,ApptViewItemL.ApptViewCur.StackBehavUR,ApptViewAlignment.UR,backBrush);
				elementI++;
			}
			//LR
			drawLoc=new Point(Width-1,Height-1);//in the LR area, we refer to the lower right corner of each element.
			elementI=ApptViewItemL.ApptRows.Count-1;//For lower right, draw the list backwards.
			while(drawLoc.Y>0 && elementI>=0) {
				if(ApptViewItemL.ApptRows[elementI].ElementAlignment!=ApptViewAlignment.LR) {
					elementI--;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,ApptViewItemL.ApptViewCur.StackBehavLR,ApptViewAlignment.LR,backBrush);
				elementI--;
			}
			//Main outline
			g.DrawRectangle(new Pen(Color.Black),0,0,Width-1,Height-1);
			//broken X
			if(DataRoww["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()){
				g.DrawLine(new Pen(Color.Black),8,1,Width-1,Height-1);
				g.DrawLine(new Pen(Color.Black),8,Height-1,Width-1,1);
			}
			this.BackgroundImage=Shadow;
			//Shadow=null;
			g.Dispose();
		}

		///<summary>Called from createShadow once for each element. The drawLoc is specified so that this method knows where to start drawing when stacking.  Returns the point where the next element is to draw based on the space that this element fills.  If stacking left or up, the drawLoc is adjusted by the width or height.</summary>
		private Point DrawElement(Graphics g,int elementI,Point drawLoc,ApptViewStackBehavior stackBehavior,ApptViewAlignment align,Brush backBrush){
			string text="";
			bool isNote=false;
			if(PIn.Long(DataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNote
				|| PIn.Long(DataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNoteCompleted) 
			{
				isNote=true;
			}
			bool isGraphic=false;
			if(ApptViewItemL.ApptRows[elementI].ElementDesc=="ConfirmedColor") {
				isGraphic=true;
			}
			if(ApptViewItemL.ApptRows[elementI].ApptFieldDefNum>0) {
				string fieldName=ApptFieldDefs.GetFieldName(ApptViewItemL.ApptRows[elementI].ApptFieldDefNum);
				for(int i=0;i<TableApptFields.Rows.Count;i++) {
					if(TableApptFields.Rows[i]["AptNum"].ToString()!=DataRoww["AptNum"].ToString()) {
						continue;
					}
					if(TableApptFields.Rows[i]["FieldName"].ToString()!=fieldName) {
						continue;
					}
					text=TableApptFields.Rows[i]["FieldValue"].ToString();
				}
			}
			else switch(ApptViewItemL.ApptRows[elementI].ElementDesc){
				case "Address":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["address"].ToString();
					}
					break;
				case "AddrNote":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["addrNote"].ToString();
					}
					break;
				case "Age":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["age"].ToString();
					}
					break;
				case "ASAP":
					if(isNote) {
						text="";
					}
					else {
						if(DataRoww["AptStatus"].ToString()==((int)ApptStatus.ASAP).ToString()){
							text=Lan.g("enumApptStatus","ASAP");
						}
					}
					break;
				case "ASAP[A]":
					if(DataRoww["AptStatus"].ToString()==((int)ApptStatus.ASAP).ToString()) {
						text="A";
					}
					break;
				case "AssistantAbbr":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["assistantAbbr"].ToString();
					}
					break;
				case "ChartNumAndName":
					text=DataRoww["chartNumAndName"].ToString();
					break;
				case "ChartNumber":
					text=DataRoww["chartNumber"].ToString();
					break;
				case "CreditType":
					text=DataRoww["CreditType"].ToString();
					break;
				case "Guardians":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["guardians"].ToString();
					}
					break;
				case "HasIns[I]":
					text=DataRoww["hasIns[I]"].ToString();
					break;
				case "HmPhone":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["hmPhone"].ToString();
					}
					break;
				case "InsToSend[!]":
					text=DataRoww["insToSend[!]"].ToString();
					break;
				case "Lab":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["lab"].ToString();
					}
					break;
				case "MedOrPremed[+]":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["medOrPremed[+]"].ToString();
					}
					break;
				case "MedUrgNote":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["MedUrgNote"].ToString();
					}
					break;
				case "Note":
					text=DataRoww["Note"].ToString();
					break;
				case "PatientName":
					text=DataRoww["patientName"].ToString();
					break;
				case "PatientNameF":
					text=DataRoww["patientNameF"].ToString();
					break;
				case "PatNum":
					text=DataRoww["patNum"].ToString();
					break;
				case "PatNumAndName":
					text=DataRoww["patNumAndName"].ToString();
					break;
				case "PremedFlag":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["preMedFlag"].ToString();
					}
					break;
				case "Procs":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["procs"].ToString();
					}
					break;
				case "Production":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["production"].ToString();
					}
					break;
				case "Provider":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["provider"].ToString();
					}
					break;
				case "WirelessPhone":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["wirelessPhone"].ToString();
					}
					break;
				case "WkPhone":
					if(isNote) {
						text="";
					}
					else {
						text=DataRoww["wkPhone"].ToString();
					}
					break;
				
			}
			if(text=="" && !isGraphic) {
				return drawLoc;//next element will draw at the same position as this one would have.
			}
			SolidBrush brush=new SolidBrush(ApptViewItemL.ApptRows[elementI].ElementColor);
			SolidBrush brushWhite=new SolidBrush(Color.White);
			SolidBrush noteTitlebrush = new SolidBrush(DefC.Long[(int)DefCat.AppointmentColors][8].ItemColor);
			StringFormat format=new StringFormat();
			format.Alignment=StringAlignment.Near;
			int charactersFitted;//not used, but required as 'out' param for measureString.
			int linesFilled;
			SizeF noteSize;
			RectangleF rect;
			RectangleF rectBack;
			if(align==ApptViewAlignment.Main) {//always stacks vertical
				if(isGraphic) {
					Bitmap bitmap=new Bitmap(12,12);
					noteSize=new SizeF(bitmap.Width,bitmap.Height);
					rect=new RectangleF(drawLoc,noteSize);
					using(Graphics gfx=Graphics.FromImage(bitmap)){
						gfx.SmoothingMode=SmoothingMode.HighQuality;
						Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(DataRoww["Confirmed"].ToString()));
						SolidBrush confirmBrush=new SolidBrush(confirmColor);
						gfx.FillEllipse(confirmBrush,0,0,11,11);
						gfx.DrawEllipse(Pens.Black,0,0,11,11);
						confirmBrush.Dispose();
					}
					g.DrawImage(bitmap,drawLoc.X,drawLoc.Y);
					return new Point(drawLoc.X,drawLoc.Y+(int)noteSize.Height);
				}
				else {
					noteSize=g.MeasureString(text,baseFont,Width-9);
					g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
					rect=new RectangleF(drawLoc,noteSize);
					g.DrawString(text,baseFont,brush,rect,format);
					return new Point(drawLoc.X,drawLoc.Y+linesFilled*ContrApptSheet.Lh);
				}
			}
			else if(align==ApptViewAlignment.UR) {
				if(stackBehavior==ApptViewStackBehavior.Vertical) {
					int w=Width-9;
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(DataRoww["Confirmed"].ToString()));
							SolidBrush confirmBrush=new SolidBrush(confirmColor);
							gfx.FillEllipse(confirmBrush,0,0,11,11);
							gfx.DrawEllipse(Pens.Black,0,0,11,11);
							confirmBrush.Dispose();
						}
						g.DrawImage(bitmap,drawLocThis.X,drawLocThis.Y);
						return new Point(drawLoc.X,drawLoc.Y+(int)noteSize.Height);
					}
					else {
						noteSize=g.MeasureString(text,baseFont,w);
						noteSize=new SizeF(noteSize.Width,ContrApptSheet.Lh+1);//only allowed to be one line high.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,ContrApptSheet.Lh);
						if(ApptViewItemL.ApptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="HasIns[I]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="InsToSend[!]")
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else{
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X,drawLoc.Y+ContrApptSheet.Lh);//move down a certain number of lines for next element.
					}
				}
				else {//horizontal
					int w=drawLoc.X-9;//drawLoc is upper right of each element.  The first element draws at (Width-1,0).
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(DataRoww["Confirmed"].ToString()));
							SolidBrush confirmBrush=new SolidBrush(confirmColor);
							gfx.FillEllipse(confirmBrush,0,0,11,11);
							gfx.DrawEllipse(Pens.Black,0,0,11,11);
							confirmBrush.Dispose();
						}
						g.DrawImage(bitmap,drawLocThis.X,drawLocThis.Y);
						return new Point(drawLoc.X-(int)noteSize.Width-2,drawLoc.Y);
					}
					else {
						noteSize=g.MeasureString(text,baseFont,w);
						noteSize=new SizeF(noteSize.Width,ContrApptSheet.Lh+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,ContrApptSheet.Lh);
						if(ApptViewItemL.ApptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="HasIns[I]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="InsToSend[!]")
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else{
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X-(int)noteSize.Width-1,drawLoc.Y);//Move to left.  Might also have to subtract a little from x to space out elements.
					}
				}
			}
			else {//LR
				if(stackBehavior==ApptViewStackBehavior.Vertical) {
					int w=Width-9;
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1-ContrApptSheet.Lh);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(DataRoww["Confirmed"].ToString()));
							SolidBrush confirmBrush=new SolidBrush(confirmColor);
							gfx.FillEllipse(confirmBrush,0,0,11,11);
							gfx.DrawEllipse(Pens.Black,0,0,11,11);
							confirmBrush.Dispose();
						}
						g.DrawImage(bitmap,drawLocThis.X,drawLocThis.Y);
						return new Point(drawLoc.X,drawLoc.Y-(int)noteSize.Height);
					}
					else {
						noteSize=g.MeasureString(text,baseFont,w);
						noteSize=new SizeF(noteSize.Width,ContrApptSheet.Lh+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y-ContrApptSheet.Lh);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,ContrApptSheet.Lh);
						if(ApptViewItemL.ApptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="HasIns[I]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="InsToSend[!]")
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else{
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X,drawLoc.Y-ContrApptSheet.Lh);//move up a certain number of lines for next element.
					}
				}
				else {//horizontal
					int w=drawLoc.X-9;//drawLoc is upper right of each element.  The first element draws at (Width-1,0).
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width+1,drawLoc.Y+1-ContrApptSheet.Lh);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(DataRoww["Confirmed"].ToString()));
							SolidBrush confirmBrush=new SolidBrush(confirmColor);
							gfx.FillEllipse(confirmBrush,0,0,11,11);
							gfx.DrawEllipse(Pens.Black,0,0,11,11);
							confirmBrush.Dispose();
						}
						g.DrawImage(bitmap,drawLocThis.X,drawLocThis.Y);
						return new Point(drawLoc.X-(int)noteSize.Width-1,drawLoc.Y);
					}
					else {
						noteSize=g.MeasureString(text,baseFont,w);
						noteSize=new SizeF(noteSize.Width,ContrApptSheet.Lh+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y-ContrApptSheet.Lh);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,ContrApptSheet.Lh);
						if(ApptViewItemL.ApptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="HasIns[I]"
							|| ApptViewItemL.ApptRows[elementI].ElementDesc=="InsToSend[!]")
						{
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else{
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X-(int)noteSize.Width-1,drawLoc.Y);//Move to left.  Subtract a little from x to space out elements.
					}
				}
			}
		}

		private void ContrApptSingle_Load(object sender, System.EventArgs e){
			/*
			if(Info.IsNext){
				Width=110;
				//don't change location
			}
			else{
				Location=new Point(ConvertToX(),ConvertToY());
				Width=ContrApptSheet.ColWidth-1;
				Height=Info.MyApt.Pattern.Length*ContrApptSheet.Lh;
			}
			*/
		}

		private void ContrApptSingle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			ClickedAptNum=PIn.Long(DataRoww["AptNum"].ToString());
		}




	}//end class
}//end namespace

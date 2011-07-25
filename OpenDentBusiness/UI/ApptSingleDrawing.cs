﻿using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness.UI {
	public class ApptSingleDrawing {
		///<summary></summary>
		public static void DrawEntireAppt(Graphics g,DataRow dataRoww,int totalWidth,int totalHeight,string patternShowing,int lineH,int rowsPerIncr,bool isSelected,bool thisIsPinBoard,long selectedAptNum,List<ApptViewItem> apptRows,ApptView apptViewCur,DataTable tableApptFields,DataTable tablePatFields) {
			Pen penB=new Pen(Color.Black);
			Pen penW=new Pen(Color.White);
			Pen penGr=new Pen(Color.SlateGray);
			Pen penDG=new Pen(Color.DarkSlateGray);
			Pen penO;//provider outline color
			Color backColor;
			Color provColor;
			if(dataRoww["ProvNum"].ToString()!="0" && dataRoww["IsHygiene"].ToString()=="0") {//dentist
				provColor=Providers.GetColor(PIn.Long(dataRoww["ProvNum"].ToString()));
				penO=new Pen(Providers.GetOutlineColor(PIn.Long(dataRoww["ProvNum"].ToString())));
			}
			else if(dataRoww["ProvHyg"].ToString()!="0" && dataRoww["IsHygiene"].ToString()=="1") {//hygienist
				provColor=Providers.GetColor(PIn.Long(dataRoww["ProvHyg"].ToString()));
				penO=new Pen(Providers.GetOutlineColor(PIn.Long(dataRoww["ProvHyg"].ToString())));
			}
			else {//unknown
				provColor=Color.White;
				penO=new Pen(Color.Black);
			}
			if(PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.Complete) {
				backColor=DefC.Long[(int)DefCat.AppointmentColors][3].ItemColor;
			}
			else if(PIn.Long(dataRoww["AptStatus"].ToString())==(int)ApptStatus.PtNote) {
				backColor=DefC.Long[(int)DefCat.AppointmentColors][7].ItemColor;
			}
			else if(PIn.Long(dataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNoteCompleted) {
				backColor = DefC.Long[(int)DefCat.AppointmentColors][10].ItemColor;
			}
			else {
				backColor=provColor;
				//We might want to do something interesting here.
			}
			SolidBrush backBrush=new SolidBrush(backColor);
			g.FillRectangle(backBrush,7,0,totalWidth-7,totalHeight);
			g.FillRectangle(Brushes.White,0,0,7,totalHeight);
			g.DrawLine(penB,7,0,7,totalHeight);
			Pen penTimediv=Pens.Silver;
			//g.TextRenderingHint=TextRenderingHint.SingleBitPerPixelGridFit;//to make printing clearer
			for(int i=0;i<patternShowing.Length;i++) {//Info.MyApt.Pattern.Length;i++){
				if(patternShowing.Substring(i,1)=="X") {
					g.FillRectangle(new SolidBrush(provColor),1,i*lineH+1,6,lineH);
				}
				else {
					//leave empty
				}
				if(Math.IEEERemainder((double)i,(double)rowsPerIncr)==0) {//0/1
					g.DrawLine(penTimediv,1,i*lineH,6,i*lineH);
				}
			}
			//Highlighting border
			if(isSelected	|| (!thisIsPinBoard && dataRoww["AptNum"].ToString()==selectedAptNum.ToString())) {
				//Left
				g.DrawLine(penO,8,1,8,totalHeight-2);
				g.DrawLine(penO,9,1,9,totalHeight-3);
				//Right
				g.DrawLine(penO,totalWidth-2,1,totalWidth-2,totalHeight-2);
				g.DrawLine(penO,totalWidth-3,2,totalWidth-3,totalHeight-3);
				//Top
				g.DrawLine(penO,8,1,totalWidth-2,1);
				g.DrawLine(penO,8,2,totalWidth-3,2);
				//bottom
				g.DrawLine(penO,9,totalHeight-2,totalWidth-2,totalHeight-2);
				g.DrawLine(penO,10,totalHeight-3,totalWidth-3,totalHeight-3);
			}
			//Draw all the main rows
			Point drawLoc=new Point(9,0);
			int elementI=0;
			while(drawLoc.Y<totalHeight && elementI<apptRows.Count) {
				if(apptRows[elementI].ElementAlignment!=ApptViewAlignment.Main) {
					elementI++;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,ApptViewStackBehavior.Vertical,ApptViewAlignment.Main,backBrush,dataRoww,apptRows,tableApptFields,tablePatFields,lineH,totalWidth);//set the drawLoc to a new point, based on space used by element
				elementI++;
			}
			//UR
			drawLoc=new Point(totalWidth-1,0);//in the UR area, we refer to the upper right corner of each element.
			elementI=0;
			while(drawLoc.Y<totalHeight && elementI<apptRows.Count) {
				if(apptRows[elementI].ElementAlignment!=ApptViewAlignment.UR) {
					elementI++;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,apptViewCur.StackBehavUR,ApptViewAlignment.UR,backBrush,dataRoww,apptRows,tableApptFields,tablePatFields,lineH,totalWidth);
				elementI++;
			}
			//LR
			drawLoc=new Point(totalWidth-1,totalHeight-1);//in the LR area, we refer to the lower right corner of each element.
			elementI=apptRows.Count-1;//For lower right, draw the list backwards.
			while(drawLoc.Y>0 && elementI>=0) {
				if(apptRows[elementI].ElementAlignment!=ApptViewAlignment.LR) {
					elementI--;
					continue;
				}
				drawLoc=DrawElement(g,elementI,drawLoc,apptViewCur.StackBehavLR,ApptViewAlignment.LR,backBrush,dataRoww,apptRows,tableApptFields,tablePatFields,lineH,totalWidth);
				elementI--;
			}
			//Main outline
			g.DrawRectangle(new Pen(Color.Black),0,0,totalWidth-1,totalHeight-1);
			//broken X
			if(dataRoww["AptStatus"].ToString()==((int)ApptStatus.Broken).ToString()) {
				g.DrawLine(new Pen(Color.Black),8,1,totalWidth-1,totalHeight-1);
				g.DrawLine(new Pen(Color.Black),8,totalHeight-1,totalWidth-1,1);
			}
		}

		///<summary></summary>
		public static Point DrawElement(Graphics g,int elementI,Point drawLoc,ApptViewStackBehavior stackBehavior,ApptViewAlignment align,Brush backBrush,DataRow dataRoww,List<ApptViewItem> apptRows,DataTable tableApptFields,DataTable tablePatFields,int lineH,int totalWidth) {
			Font baseFont=new Font("Arial",8);
			string text="";
			bool isNote=false;
			if(PIn.Long(dataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNote
				|| PIn.Long(dataRoww["AptStatus"].ToString()) == (int)ApptStatus.PtNoteCompleted) {
				isNote=true;
			}
			bool isGraphic=false;
			if(apptRows[elementI].ElementDesc=="ConfirmedColor") {
				isGraphic=true;
			}
			if(apptRows[elementI].ApptFieldDefNum>0) {
				string fieldName=ApptFieldDefs.GetFieldName(apptRows[elementI].ApptFieldDefNum);
				for(int i=0;i<tableApptFields.Rows.Count;i++) {
					if(tableApptFields.Rows[i]["AptNum"].ToString()!=dataRoww["AptNum"].ToString()) {
						continue;
					}
					if(tableApptFields.Rows[i]["FieldName"].ToString()!=fieldName) {
						continue;
					}
					text=tableApptFields.Rows[i]["FieldValue"].ToString();
				}
			}
			else if(apptRows[elementI].PatFieldDefNum>0) {
				string fieldName=PatFieldDefs.GetFieldName(apptRows[elementI].PatFieldDefNum);
				for(int i=0;i<tablePatFields.Rows.Count;i++) {
					if(tablePatFields.Rows[i]["PatNum"].ToString()!=dataRoww["PatNum"].ToString()) {
						continue;
					}
					if(tablePatFields.Rows[i]["FieldName"].ToString()!=fieldName) {
						continue;
					}
					text=tablePatFields.Rows[i]["FieldValue"].ToString();
				}
			}
			else switch(apptRows[elementI].ElementDesc) {
					case "Address":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["address"].ToString();
						}
						break;
					case "AddrNote":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["addrNote"].ToString();
						}
						break;
					case "Age":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["age"].ToString();
						}
						break;
					case "ASAP":
						if(isNote) {
							text="";
						}
						else {
							if(dataRoww["AptStatus"].ToString()==((int)ApptStatus.ASAP).ToString()) {
								text=Lans.g("enumApptStatus","ASAP");
							}
						}
						break;
					case "ASAP[A]":
						if(dataRoww["AptStatus"].ToString()==((int)ApptStatus.ASAP).ToString()) {
							text="A";
						}
						break;
					case "AssistantAbbr":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["assistantAbbr"].ToString();
						}
						break;
					case "ChartNumAndName":
						text=dataRoww["chartNumAndName"].ToString();
						break;
					case "ChartNumber":
						text=dataRoww["chartNumber"].ToString();
						break;
					case "CreditType":
						text=dataRoww["CreditType"].ToString();
						break;
					case "Guardians":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["guardians"].ToString();
						}
						break;
					case "HasIns[I]":
						text=dataRoww["hasIns[I]"].ToString();
						break;
					case "HmPhone":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["hmPhone"].ToString();
						}
						break;
					case "InsToSend[!]":
						text=dataRoww["insToSend[!]"].ToString();
						break;
					case "Lab":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["lab"].ToString();
						}
						break;
					case "MedOrPremed[+]":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["medOrPremed[+]"].ToString();
						}
						break;
					case "MedUrgNote":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["MedUrgNote"].ToString();
						}
						break;
					case "Note":
						text=dataRoww["Note"].ToString();
						break;
					case "PatientName":
						text=dataRoww["patientName"].ToString();
						break;
					case "PatientNameF":
						text=dataRoww["patientNameF"].ToString();
						break;
					case "PatNum":
						text=dataRoww["patNum"].ToString();
						break;
					case "PatNumAndName":
						text=dataRoww["patNumAndName"].ToString();
						break;
					case "PremedFlag":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["preMedFlag"].ToString();
						}
						break;
					case "Procs":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["procs"].ToString();
						}
						break;
					case "ProcsColored":
						string value=dataRoww["procsColored"].ToString();
						string[] lines=value.Split(new string[] { "</span>" },StringSplitOptions.RemoveEmptyEntries);
						Point tempPt=new Point();
						tempPt=drawLoc;
						int lastH=0;
						int count=1;
						for(int i=0;i<lines.Length;i++) {
							Match m=Regex.Match(lines[i],"^<span color=\"(-?[0-9]*)\">(.*)$");
							string rgbInt=m.Result("$1");
							string proc=m.Result("$2");
							if(lines[i]!=lines[lines.Length-1]) {
								proc+=",";
							}
							if(rgbInt=="") {
								rgbInt=apptRows[elementI].ElementColorXml.ToString();
							}
							Color c=Color.FromArgb(Convert.ToInt32(rgbInt));
							StringFormat procFormat=new StringFormat();
							RectangleF procRect=new RectangleF(0,0,1000,1000);
							CharacterRange[] ranges= { new CharacterRange(0,proc.Length) };
							Region[] regions=new Region[1];
							procFormat.SetMeasurableCharacterRanges(ranges);
							regions=g.MeasureCharacterRanges(proc,baseFont,procRect,procFormat);
							if(regions.Length==0) {
								procRect=new RectangleF(0,0,0,0);
							}
							else {
								procRect=regions[0].GetBounds(g);
							}
							if(tempPt.X+procRect.Width>totalWidth) {
								tempPt.X=drawLoc.X;
								tempPt.Y+=lastH;
								count++;
							}
							SolidBrush sb=new SolidBrush(c);
							g.DrawString(proc,baseFont,sb,tempPt);
							sb.Dispose();
							tempPt.X+=(int)procRect.Width+3;//+3 is room for spaces
							if((int)procRect.Height>lastH) {
								lastH=(int)procRect.Height;
							}
						}
						drawLoc.Y+=lastH*count;
						return drawLoc;
					case "Production":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["production"].ToString();
						}
						break;
					case "Provider":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["provider"].ToString();
						}
						break;
					case "TimeAskedToArrive":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["timeAskedToArrive"].ToString();//could be blank
						}
						break;
					case "WirelessPhone":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["wirelessPhone"].ToString();
						}
						break;
					case "WkPhone":
						if(isNote) {
							text="";
						}
						else {
							text=dataRoww["wkPhone"].ToString();
						}
						break;

				}
			if(text=="" && !isGraphic) {
				return drawLoc;//next element will draw at the same position as this one would have.
			}
			SolidBrush brush=new SolidBrush(apptRows[elementI].ElementColor);
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
					using(Graphics gfx=Graphics.FromImage(bitmap)) {
						gfx.SmoothingMode=SmoothingMode.HighQuality;
						Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRoww["Confirmed"].ToString()));
						SolidBrush confirmBrush=new SolidBrush(confirmColor);
						gfx.FillEllipse(confirmBrush,0,0,11,11);
						gfx.DrawEllipse(Pens.Black,0,0,11,11);
						confirmBrush.Dispose();
					}
					g.DrawImage(bitmap,drawLoc.X,drawLoc.Y);
					return new Point(drawLoc.X,drawLoc.Y+(int)noteSize.Height);
				}
				else {
					noteSize=g.MeasureString(text,baseFont,totalWidth-9);
					//Problem: "limited-tooth bothering him ", the trailing space causes measuring error, resulting in m getting partially chopped off.
					//Tried TextRenderer, but it caused premature wrapping
					//Size noteSizeInt=TextRenderer.MeasureText(text,baseFont,new Size(totalWidth-9,1000));
					//noteSize=new SizeF(noteSizeInt.totalWidth,noteSizeInt.totalHeight);
					noteSize.Width=(float)Math.Ceiling((double)noteSize.Width);//round up to nearest int solves specific problem discussed above.
					g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
					rect=new RectangleF(drawLoc,noteSize);
					g.DrawString(text,baseFont,brush,rect,format);
					return new Point(drawLoc.X,drawLoc.Y+linesFilled*lineH);
				}
			}
			else if(align==ApptViewAlignment.UR) {
				if(stackBehavior==ApptViewStackBehavior.Vertical) {
					int w=totalWidth-9;
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRoww["Confirmed"].ToString()));
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
						noteSize=new SizeF(noteSize.Width,lineH+1);//only allowed to be one line high.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,lineH);
						if(apptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| apptRows[elementI].ElementDesc=="HasIns[I]"
							|| apptRows[elementI].ElementDesc=="InsToSend[!]") {
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else {
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X,drawLoc.Y+lineH);//move down a certain number of lines for next element.
					}
				}
				else {//horizontal
					int w=drawLoc.X-9;//drawLoc is upper right of each element.  The first element draws at (totalWidth-1,0).
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRoww["Confirmed"].ToString()));
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
						noteSize=new SizeF(noteSize.Width,lineH+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,lineH);
						if(apptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| apptRows[elementI].ElementDesc=="HasIns[I]"
							|| apptRows[elementI].ElementDesc=="InsToSend[!]") {
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else {
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
					int w=totalWidth-9;
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y+1-lineH);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRoww["Confirmed"].ToString()));
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
						noteSize=new SizeF(noteSize.Width,lineH+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						//g.MeasureString(text,baseFont,noteSize,format,out charactersFitted,out linesFilled);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y-lineH);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,lineH);
						if(apptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| apptRows[elementI].ElementDesc=="HasIns[I]"
							|| apptRows[elementI].ElementDesc=="InsToSend[!]") {
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else {
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X,drawLoc.Y-lineH);//move up a certain number of lines for next element.
					}
				}
				else {//horizontal
					int w=drawLoc.X-9;//drawLoc is upper right of each element.  The first element draws at (totalWidth-1,0).
					if(isGraphic) {
						Bitmap bitmap=new Bitmap(12,12);
						noteSize=new SizeF(bitmap.Width,bitmap.Height);
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width+1,drawLoc.Y+1-lineH);//upper left corner of this element
						rect=new RectangleF(drawLoc,noteSize);
						using(Graphics gfx=Graphics.FromImage(bitmap)) {
							gfx.SmoothingMode=SmoothingMode.HighQuality;
							Color confirmColor=DefC.GetColor(DefCat.ApptConfirmed,PIn.Long(dataRoww["Confirmed"].ToString()));
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
						noteSize=new SizeF(noteSize.Width,lineH+1);//only allowed to be one line high.  Needs an extra pixel.
						if(noteSize.Width<5) {
							noteSize=new SizeF(5,noteSize.Height);
						}
						Point drawLocThis=new Point(drawLoc.X-(int)noteSize.Width,drawLoc.Y-lineH);//upper left corner of this element
						rect=new RectangleF(drawLocThis,noteSize);
						rectBack=new RectangleF(drawLocThis.X,drawLocThis.Y+1,noteSize.Width,lineH);
						if(apptRows[elementI].ElementDesc=="MedOrPremed[+]"
							|| apptRows[elementI].ElementDesc=="HasIns[I]"
							|| apptRows[elementI].ElementDesc=="InsToSend[!]") {
							g.FillRectangle(brush,rectBack);
							g.DrawString(text,baseFont,Brushes.Black,rect,format);
						}
						else {
							g.FillRectangle(brushWhite,rectBack);
							g.DrawString(text,baseFont,brush,rect,format);
						}
						g.DrawRectangle(Pens.Black,rectBack.X,rectBack.Y,rectBack.Width,rectBack.Height);
						return new Point(drawLoc.X-(int)noteSize.Width-1,drawLoc.Y);//Move to left.  Subtract a little from x to space out elements.
					}
				}
			}
		}


	}
}
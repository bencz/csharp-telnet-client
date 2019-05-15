using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoCoder.Reflection;
using AutoCoder.File;
using AutoCoder.Text;
using AutoCoder.Parse;
using AutoCoder.Print;
using AutoCoder.Core;
using AutoCoder.Scan;

namespace Tester
{
  public partial class Form1 : Form
  {
    ListBox lbResults = null;
    MenuStrip msMenu = null;

    public Form1()
    {
      InitializeComponent();
      BuildForm();
    }

    void BuildForm()
    {
      lbResults = new ListBox();
      lbResults.Dock = DockStyle.Fill;
      lbResults.AutoSize = true;
      lbResults.Parent = this;
      lbResults.Font = new Font("Lucida Console", 9);
      lbResults.HorizontalScrollbar = true;

      msMenu = new MenuStrip();
      msMenu.Parent = this;
      msMenu.Items.Add("Test", null, menu_Test_Click);
      msMenu.Items.Add("Parse", null, menu_Parse_Click);
      msMenu.Items.Add("Print", null, menu_Print_Click);
      msMenu.Items.Add("Exit", null, menu_Exit_Click);

    }

    void menu_Exit_Click(object InObj, EventArgs InArgs)
    {
      this.Close();
    }

    void menu_Parse_Click(object InObj, EventArgs InArgs)
    {
      string stmtText = null;

      StmtTraits traits = new StmtTraits();
      traits.BracedTreatment = ScannerBracedTreatment.Parts;
      traits.OpenNamedBracedPatterns.Clear( ) ;
      traits.OpenContentBracedPatterns.Replace(new string[] { "(", "[", "<", "{" }) ;
      traits.FormSentencesFromWhitespaceDelimWords = true;
      traits.EndStmtPatterns.Replace(";" ) ;
      traits.CommentToEndPatterns.AddDistinct("--") ;
      traits.NewLineIsWhitespace = true;

      stmtText =
        "create table acctnotep( CustName char(30), CustNbr decimal(7,0) )  ;";
      stmtText = 
        "create (table), acctnotep( CustName timestamp, " + 
        "CustNbr char(25) ) ;";
      
      string line1 =
        "-- name       : acctnotep";
      string line1b = "abc, efg, oct, nov";
      string line2 =
        "create table  acctnotep(" ;
      string line3 =
        "sn            char(6) not null,  -- speed number ";
      string line4 = 
        "primary key( sn, mbrnu, textSeqn )" ;
      string line5 =
        ") steve ;" ;
      string[] stmtTextArray = new string[] { stmtText, line1, line1b, line2, line3, line4, line5 };

      // build a complex that contains the lines to parse concatenated together.
      // The complex also a cross reference for converting buffer locations to
      // line positions.
      ParseBufferComplex buf = new ParseBufferComplex(stmtTextArray);
      
      StmtWord topWord = StmtParser.ParseTextLines(buf, traits );

      ShowParsedWords(topWord);
    }

    void menu_Print_Click(object InObj, EventArgs InArgs)
    {
      if (lbResults.Items.Count > 0)
      {
        string[] lines = new string[lbResults.Items.Count];
        int ix = 0;
        foreach (string lbItem in lbResults.Items)
        {
          lines[ix] = lbItem;
          ix += 1;
        }
        LinePrinter.PrintLines(lines, 0);
      }
    }

    void menu_Test_Click(object InObj, EventArgs InArgs)
    {

      string s1 = "+++,abc,&efg,&&&";
      string s2 = s1.SplitCharEncode(',', "&+");
      string s3 = s2.SplitCharDecode(',');
      MessageBox.Show(s1 + Environment.NewLine +
        s2 + Environment.NewLine +
        s3);



#if _NotUsed
      int xx = 25;
      string bb = "25";
      Type tp4 = bb.GetType();
      if ( tp4.IsPrimitiveOrString( ) == true )
        xx = 22 ;

      Size sx = new Size( ) ;
      Type t5 = sx.GetType( ) ;
      if ( t5.IsPrimitive == true )
        xx = 88 ;

      FrameworkType fw1 = new FrameworkType(TypeEnum.Int);
      Type tp1 = fw1.GetSystemType();

      FrameworkType fw2 = new FrameworkType(TypeEnum.String);
      Type tp2 = fw2.GetSystemType();

      FrameworkType fw3 = new FrameworkType("Size");
      Type tp3 = fw3.GetSystemType();
#endif

    }

    // ----------------------- ShowParsedWords -------------------------------
    void ShowParsedWords(StmtWord InTopWord)
    {
      ParsedWordsReport rep = ParsedWordsReport.ReportParsedWords(InTopWord);
      foreach (string s1 in rep)
      {
        lbResults.Items.Add(s1);
      }
    }

  }
}


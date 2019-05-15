using AutoCoder.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Models
{
  public class ReportLineModel : HierarchicalModelBase
  {
    public string LineText
    { get; set; }

    public ReportLineModel(string LineText)
    {
      this.LineText = LineText;
    }

    public override bool CanExpand
    {
      get
      {
        return false;
      }
    }

    protected override void FillChildren()
    {
    }

    protected override void FillExpandableMarker()
    {
    }
  }
}

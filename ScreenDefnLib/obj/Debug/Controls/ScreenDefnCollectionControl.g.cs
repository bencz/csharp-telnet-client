﻿#pragma checksum "..\..\..\Controls\ScreenDefnCollectionControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FF7FBD835A3C9D2B2B18F8E5B268EA35"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ScreenDefnLib.Controls;
using ScreenDefnLib.Converters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace ScreenDefnLib.Controls {
    
    
    /// <summary>
    /// ScreenDefnCollectionControl
    /// </summary>
    public partial class ScreenDefnCollectionControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid LayoutRoot;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button butAdd;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvScreenDefn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ScreenDefnLib;component/controls/screendefncollectioncontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.LayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.butAdd = ((System.Windows.Controls.Button)(target));
            
            #line 30 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            this.butAdd.Click += new System.Windows.RoutedEventHandler(this.butAdd_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lvScreenDefn = ((System.Windows.Controls.ListView)(target));
            
            #line 34 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            this.lvScreenDefn.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.lvScreenDefn_MouseDoubleClick);
            
            #line default
            #line hidden
            
            #line 35 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            this.lvScreenDefn.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lvScreenDefn_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 49 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 50 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 51 "..\..\..\Controls\ScreenDefnCollectionControl.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MenuItem_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

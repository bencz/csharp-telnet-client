   M S B A M L   `   `   `  ����W  SAutoCoder, Version=1.0.7074.18330, Culture=neutral, PublicKeyToken=f834fd727b4fa2b1J3clr-namespace:AutoCoder.Controls;assembly=AutoCoderAutoCoder.Controls  5   	   C ?tnClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:tnClient.WindowstnClient.Windows 5   	   *   $tnClient.Windows.CaptureViewerWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Dacuc3clr-namespace:AutoCoder.Controls;assembly=AutoCoder     5   	   0localclr-namespace:tnClient.Windows     5   	     � Title$  CaptureViewerWindow��5	   	   $	��350��6%   $	��725��62   .��5
      � -   5         �Name$ 
LayoutRoot��5
       � RowDefinitions 5      �� 5   
   $
��auto �6   6(   �� 5   
   $��* �6   6%   �� 5   
   $
��auto �6   6(    � ColumnDefinitions 5      �� 5      $��* �6   .+�5      o� -   5        $��menu1��5      $
��Left=�5      $��0��6)   $	��Top=�5      .@�5   
   m� -   5        $
��Test��5      6:   m� 5   
   -   5        $
��Exit��5      6:   5   	   � 5      $��1��6    5      �� 5   
   $
��auto �6   �� 5   
   $��* �6    5!      �� 5"   
   $
��auto �6   �� 5#   
   $��* �6   .+�5&      �� $��0��6   $��0��6   $��Center=�5'      .��6/   Capture folder:6@   1   +AutoCoder.Controls.FolderPathTextBoxControl  5(      $��0��5)      $��1��6    $��Center=�5*       $��Stretch=�5+          FolderPathText 5,       �� *CaptureFolderPath+� 5.      $��1��6   $��0��6   $��2��6*    5/   
   �� 50      $��* �6   6'    52   
   �� 53      $	��160 �6   6+   �� 54      $��2 �6   6)   �� 55      $��* �6   6)   .+�58   
   �� -   5         ��Name$ lbLog��58      ��5:      �� *RunLog+ �� View 5;      �� 5<      .P�5=      ��  �� Width$	 100��6   $��	File name��6+    �� DisplayMemberBinding 5>   %   �� *
fileName+�� 5?      $ 80��6   $��Create date/time��6*    5@   %   �� *	crtDate+5A      5C      �� 5G   
   
����  �$��1��6-   $��0��6=   $
��Auto��5H      $
��Auto��6!   $��Stretch=�5I      $��Stretch=�61   �� 5J   
   $��0��6   $��2��6"   .@�5L      �� $��dataGrid��6   .��5M      &   System.Windows.Controls.DataGrid  ��6   �� *CaptureDataAsDataTable+5N      5O      �� 5Q      $	��doc��6   .��5R      �� -   5        	 ��Name$
	 rtb1��5R   7   $��0��6   $��2��6'   
 �� IsDocumentEnabled
 . . �5S      /�  /� ColumnWidth$	 400��6   .T�5T      J� .*�6   .*�5U      �� �6   �� *SelectedFileContent+5V      5W      5X      5Y      �� 5[      -   5         ��Name$ TabItem2��5[      $��ListView��6&   5\      5^      5`   	   5a      5c      5d      
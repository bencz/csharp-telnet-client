   M S B A M L   `   `   `  ����W  SAutoCoder, Version=1.0.6183.10759, Culture=neutral, PublicKeyToken=f834fd727b4fa2b1J3clr-namespace:AutoCoder.Controls;assembly=AutoCoderAutoCoder.Controls  5   	   C ?tnClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:tnClient.ControlstnClient.Controls 5      -   'tnClient.Controls.ClientSettingsControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      Dacuc3clr-namespace:AutoCoder.Controls;assembly=AutoCoder     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:tnClient.Controls     5      .��5
      � -   5          �Name$  
LayoutRoot��5
       � 	Resources 5       �� 
TargetType �� Property �� Value��%}   '��       �� 5      " ��6   .�5   
   ��  � ��$ 2,0,4,0q�6#   �� 5   
    � ��$
 Bold"�6'   5   	   5       � RowDefinitions 5      �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6    � ColumnDefinitions 5!      �� 5"      $
��Auto �6   �� 5#      $��* �6   .+�5&      �� $��0��6   .��6   Connect to system6,   �� 5'      $��0��6   $��1��6   $	��120��6-   $
��Left=�5(        �� 
IsEditable . 
���Gc�� �� SelectedItem 5*       �� *Model.SystemListSelectedItem+	 �� Text	 5+      �� *Model.SystemName+
 �� Mode$
 Default=� �� UpdateSourceTrigger$ 	LostFocus=���5,      �� *Model.SystemList+�� 5.      $��1��6   .��6   Auto connect6'   �� 5/      $��Center=�6   $��1��51      $��1��6,    d� 	IsChecked 50      ��  �� Path$ Model.AutoConnect�52      �� 54      $��2��6   $��Device name��6   )   #AutoCoder.Controls.EditableComboBox  55      $��2��6   $��1��6)   $
��Left=�56      $��Center=�69   $	��100��57         CharacterCasing$ Upper=�6-      ApplyEntryToItemsSource . ��59      �� $ Model.DeviceNameList�   	TextValue 5:      �� $ Model.DeviceName��� 5=      $��3��6   $��Terminal type��6     5>      $��3��6   $��1��6+   $
��Left=�5?      $��Center=�69   $	��100��5@      $ Upper=�6-    . ��5B      �� $ Model.TerminalTypeList� 5C      �� $ Model.TerminalType��� 5E      $��4��6   $��ODBC DSN��6   �� 5F      $��4��6   $��1��6   $
��Left=�5G      $��Center=�6)   $	��100��5H       �� CharacterCasing$ Upper=�6   ��5I      �� *Model.OdbcDsn+�� 5K      $��5��6   .��6   Screen definition path61   (   "AutoCoder.Controls.FilePathTextBox  5L      $��5��6   $��1��6(   $��Center=�5M       $	��320��6;   $
��Left=�5N          Filter$% xml Files |*.xml|All Files |*.*��5O          FilePathText 5P       �� *Model.ScreenDefnPath+�� 5R      $��6��6   .��6   Capture folder path6.   1   +AutoCoder.Controls.FolderPathTextBoxControl  5S      $��6��6$   $��1��61   $��Center=�5T       $	��320��6;   $
��Left=�5U          FolderPathText 5V       �� *Model.CaptureFolderPath+�� 5X      $��7��6   .��6   Auto capture6'   �� 5Y      $��7��6   $��1��6   $��Center=�5Z       5[      �� $ Model.CaptureAuto��� 5]      $��8��6   .��6   Run time code path6-     5^      $��8��6$   $��1��61   $��Center=�5_       $	��320��6;   $
��Left=�5`        5a       �� *Model.RunTimeCodePath+�� 5c      $��9��6   .��6   Support code file path63   �� 5d      $��9��6   $��1��6    �� Orientation$ 
Horizontal=�6.   .+�5e        $��9��6   $��1��6(   $��Center=�5f       $	��320��6;   $
��Left=�5g       $# cs Files |*.cs|All Files |*.*��5h        5i       �� *Model.SupportCodeFilePath+�� 5j      -   5         ��Name$ EditSupportCode��5j      $	��3,3q�6(   $	��3,0q�65   .��6a   Edit6g   5k      �� 5m      $��11��6   $ 
Horizontal=�6   .+�5n      �� -   5        $ butApply��5n      $��3q�6!   .��6C   Apply6J   5o      5q      5r      
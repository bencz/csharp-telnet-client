   M S B A M L   `   `   `  ����C  ?tnClient, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:tnClienttnClient  5   	   5clr-namespace:tnClient.ControlstnClient.Controls  5   	   K GAutoCoder_Telnet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_Aclr-namespace:AutoCoder.Telnet.Controls;assembly=AutoCoder_TelnetAutoCoder.Telnet.Controls 5	   	   W SAutoCoder, Version=1.0.7074.18330, Culture=neutral, PublicKeyToken=f834fd727b4fa2b1J3clr-namespace:AutoCoder.Controls;assembly=AutoCoderAutoCoder.Controls 5
   	   H DScreenDefnLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullV;clr-namespace:ScreenDefnLib.Controls;assembly=ScreenDefnLibScreenDefnLib.Controls 5   	   [?clr-namespace:AutoCoder.Telnet.Models;assembly=AutoCoder_TelnetAutoCoder.Telnet.Models 5   	       tnClient.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:tnClient     5   	   .lcclr-namespace:tnClient.Controls     5   	   PucAclr-namespace:AutoCoder.Telnet.Controls;assembly=AutoCoder_Telnet     5	   	   Dacuc3clr-namespace:AutoCoder.Controls;assembly=AutoCoder     5
   	   Ldefn;clr-namespace:ScreenDefnLib.Controls;assembly=ScreenDefnLib     5   	   Pself?clr-namespace:AutoCoder.Telnet.Models;assembly=AutoCoder_Telnet     5   	   -   5          �Name$  winMain��5   	    � Title$ 
MainWindow��5   	   $	��450��6   $	��625��6)   .��5      � -   5         �Name$ grdMain��5       � RowDefinitions 5   
   �� 5      $
��auto �6   6,   �� 5      $��* �6   6)   �� 5      $
��auto �6   6,    � ColumnDefinitions 5   
   �� 5      $��* �6   6+   .+�5      ��  �� Orientation$ 
Horizontal=�6   .+�5   
   o� -   5        $��menu1��5      $
��Left=�5      $��0��6+   $	��Top=�5      .@�5      m� -   5        $��Telnet��5      6>   m� 5   
   -   5        $��Printer��5      6=   m� 5   
   $��RunLog��6   .@�5      m� -   5        $��	Clear log��5      6A   m� 5      -   5        $��	Print log��5      6A   m� 5       -   5        $��view special��5       6D   5!      m� 5"      -	   5        $��Read xml��5"      6B   m� 5#      -
   5        $��Report canvas items��5#      6G   m� 5$   
   -   5        $��	Send data��5$      6?   m� 5%   
   -   5        $
��Test��5%      6:   m� 5&   
   $��Capture��6   .@�5'      m� ��5(      � 5)       5*      �� 5+      $
��auto �6"   62   �� 5,      $
��auto �6"   62    5.      �� 5/      $	��100 �6%   63   �� 50      $��* �6%   61   .+�53      �� $��0��6   $��0��6!   $��Center=�54      .��6/   Capture folder:6@   1  +AutoCoder.Controls.FolderPathTextBoxControl  55      $��0��56      $��1��6    $��Center=�57       $	��240��6;   $
��Left=�58          FolderPathText 59       �� *CaptureFolderPath+�� 5:      $��1��6   $��0��6%   $��Center=�5;      .��6/   Auto capture6=   �� 5<      $��1��6   $��1��6(    d� 	IsChecked 68   �� *CaptureAuto+5=      5?      m� 5@      -   5        
 � Tag$ Capture��5@   ,   ��5A      �� 5B      $ 
Horizontal=�6   .+�5C      �� $��Capture��6   �� 5D      $
��Bold"�6   $	��5,0q�6.   ��5E      �� 	 �� Path$ 	 MatchScreenDefn.ScreenName�5F      5H      m� 5I      -   5        $��View captured screens��5I      $ CaptureViewer��64   6H   5J      m� 5K   
   -   5        $
��Exit��5K      6:   5L      c� 5N      .@�5O   
   d� -   5        $��btnBold��5O   <   $ ��EditingCommands.ToggleBold��6   .��5P      �� $��Images/text_bold.png��6   $��16��60   $��16��6;   5Q      �� 5R   
   -   5        
 ��Name$
 
cbFontSize��5R      $��50��6'    �� 
IsEditable . 5S      �� 5T   
   $��5,0,2,0q�6   $��Center=�6%   	Font size6K   &   AutoCoder.Controls.NumericUpDown  5U   
   -   5          Name$ 
udFontSize��5U      61   5W   	   5X      �� 5Z      -   5         ��Name$ TabControl1��5Z      $��1��6&   $��0��63    �� SelectedIndex 5[      ��  �� ElementName$ winMain��$	 TabSelectedIndex�.@�5\      �� $��Log file��6   .��5]      (  "AutoCoder.Controls.EnhancedListBox  -   5          Name$ lbLog��5]   '    �� SelectionMode$ Extended=�5^   &   $��Lucida console)�5_   &   ��5`      �� *RunLog+6;   5a      �� 5c      $��Canvas��6   .��5d   
   �  5e      �� 5f      $��* �6   6)   �� 5g      $
��auto �6   6,   .+�5i      �� -   5         ��Name$ Canvas1��5i      
���   �|�. 6J   �� 5k      $��1��6   .@�5l      �� .��5m      �� -   5         ��Name$ tbStatusBarMessage��5m      5n      5o      5q      5r   	   �� 5t      $��Data stream��6   .��5u   
   �  5v      �� 5w      $
��auto �6   6,   �� 5x      $��* �6   6)   .+�5z      �� $ 
Horizontal=�6   .+�5{      o� -   5        $��menu2��5|      $
��Left=�5{      $��0��6/   $	��Top=�5|      .@�5}      m� -   5        $��Parse��5}      6B   m� 5~      -   5        $��wtd order detail��5~      6M   5      5�      1  +AutoCoder.Telnet.Controls.DataStreamControl  5�      -   5          Name$ DataStreamControl1��5�   .   $��1��6!      DataStreamName 5�   !   �� *DataStreamName+   	ParseText 5�   !   �� *	ParseText+5�      5�   	   �� 5�      $��Settings��6   .��5�   
   -   'tnClient.Controls.ClientSettingsControl  -   'tnClient.Controls.ClientSettingsControl   ApplySettings$) #ClientSettingsControl_ApplySettings��5�   #      Model 5�   #   �� $	 .�5�   #   5�   	   �� 5�      $��
ScreenDefn��6   .��5�   
   8  2ScreenDefnLib.Controls.ScreenDefnCollectionControl     MasterThread 5�      �� *MasterThread+   ScreenDefnObservableList 5�      �� *ScreenDefnObservableList+5�   	   �� 5�      $��Traffic��6   .��5�   
   R� -   5         R�Name$ 	tvTraffic��5�      ��6&   �� *TrafficItems+��5�      �� 5�       �� 
TargetType" Q�6   .�5�      ��  �� Property �@Q�
IsExpanded �� Value 5�   %   �� *
IsExpanded+ �� Mode$ TwoWay=��� 5�       �@Q�
IsSelected 5�   %   �� *
IsSelected+$ TwoWay=��� 5�       � ��$ Normal"�6-   �� 5�       � ��$ 12(�6+     �� Triggers  5�      P� 5�      ! P� Property! �@Q�
IsSelected" P� Value" . . �5�      ��  � ��$
 Bold"�61   5�      5�      # � 	Resources# 5�      $ �� DataType5  /AutoCoder.Telnet.Models.TrafficLogItemTreeModel% �� ItemsSource& �� Command/	  )AutoCoder.Telnet.Models.ReportHeaderModel-
  'AutoCoder.Telnet.Models.ReportLineModel��%�  (��       5�   '   *, +)(�� @    5�   '   *,	 +)(�� �    5�      *,
 +)�� 5�      "$  6'   % 5�   '   �� *
Children+.R�5�      �� $ 
Horizontal=�6   .+�5�      �� ��6   �� *	Direction+�� 5�      $	��5,0q�6   ��6)   �� *LogTimeText+�� 5�      $	��5,0q�6   �� 5�      & 6   �� *PrintCommand+.�6?   Print6F   5�      5�      5�      �� 5�      "$ 	 6'   % 5�   '   �� *
Children+.R�5�      �� $ 
Horizontal=�6   .+�5�      �� ��6   �� *Title+5�      5�      �� 5�      "$ 
 6   .R�5�      �� $ 
Horizontal=�6   .+�5�      �� $��Lucida Console)�66   ��6   �� *
LineText+66   5�      5�      5�      5�      5�   	   5�      5�      5�      
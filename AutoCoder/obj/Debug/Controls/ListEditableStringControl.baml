   M S B A M L   `   `   `  ����W  SAutoCoder, Version=1.0.7074.18329, Culture=neutral, PublicKeyToken=f834fd727b4fa2b17 clr-namespace:AutoCoder.ControlsAutoCoder.Controls  5	      2    ,AutoCoder.Controls.ListEditableStringControl   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      2local clr-namespace:AutoCoder.Controls     5	      .��5      � -   5          �Name$  
LayoutRoot��5   	   .+�5      � -   5        $��Popup1��5      $��Mouse=�5      ��.   � AllowsTransparency .  � 	StaysOpen .  .��5      �� $��1q�6    J� Opacity$	 0.8Q�6#   
�������._�5   
   �� -   5         ��Name$ 	PopupText��5      5   	   5      �� 5      $
��Auto=�6   $
��Auto=�5      .��5      �� -   5         ��Name$ ItemsControl1��5      $��Lucida console)�5      $��14(�66   ��5   ,   �� *LineCollection+��5    
   �� 5!      .R�5$      �� -   5        ��5$      ��  �� Path$ Value� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�
	 � Tag	 5&      �� $ .�
 � ContextMenu
 5'      ��  �� ElementName$ Context1��5(      
 5+   
   �� 5,      -   5         ��Name$ Context1��5,      .@�5-      m� -   5        $��Delete��5-      6'   m� 5.      -   5        $��Insert��5.      6'   5/      51   	   52      53      54      
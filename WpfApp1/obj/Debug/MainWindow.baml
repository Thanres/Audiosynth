   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   :y,http://schemas.microsoft.com/winfx/2006/xaml     5   	   R NOxyPlot.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0 oxyhttp://oxyplot.org/wpf  6@   Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5   	   !localclr-namespace:WpfApp1  5   	   $	��400��5
   	     � Title$  
MainWindow��5   	   $��492.885��6   $	��525��6-   ��5	   	   ��  �� ElementName$ image�� �� Mode$ OneWay=� �� Path$ ActualWidth�.��5      � 
���   � � RowDefinitions 5   
   �� 5      $
��277* �6   �� 5      $��24 �6   �� 5      $
��200* �6   .+�5   
   �� -   5         ��Name$ record��5   �   
���   �$��1q�6%   $��0��69   $��Center=�6G   $��0,0,0,0q�6b   
���   �._�5      �  � ColumnDefinitions 5      �� 5      $��* �6'   �� 5      $
��auto �6'   �� 5      $��* �6'   .+�5      �� -   5         ��Name$ image��5      $��1��6'   $��UniformToFill=�6V   ��67   �� *ImageSource+6V   5      5      �� 5   
   
���   �$��1q�6%   $��Center=�69   $��0��6W   $��Center=�6e   ._�5      �� $��Uniform=�62   ��6   �� *
Bodysource+62   5      � 5    
   $��1��6    5!      �� 5"      $��* �6#   �� 5#      $	��100 �6#   .+�5%      �� -   5         ��Name$ btnStart��5%      $��Start��6'   $��0��67   $��Stretch=�6G   $��Stretch=�6e   $
��Auto��6�   $
��Auto��6�   ���#AAA6�   �� 5&      -   5        $ 
btnrectype��5&      $��Use Savedata��6)   $��1��6@   $��Stretch=�6P   $��Stretch=�6n   $
��Auto��6�   $
��Auto��6�   ���#AAA6�   5'      �� 5(   
   $��2��6   $��0,0,0,0q�6   ���#555555._�5)        OxyPlot.Wpf.PlotView  $��0,0,25,0q�66   
������ 	   Model	 6   �� *
DataPlot+6H   5*      5+      5,      
   M S B A M L   `   `   `  ����  ColorToDepth_KinectV1.8)    #ColorToDepth_KinectV1._8.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Picking Distance��5      $	��800��6   $
��1000��6+   .��5      � .+�5   
   �� -   5        $��camera��5   @   $	��640��6N   $	��480��6Z   $
��Left=�6h   $	��Top=�6�   �� 5	   
   -   5        $��depth��5	   @   $	��320��6M   $	��240��6Y   $��Bottom=�6g   $
��Left=�6�   �� 5
   
   -   5        $��colorCanvas��5
      $	��640��6$   $	��480��60   $
��Left=�6=   $	��Top=�6X   6s   �� 5   
   -   5        $��depthCanvas��5      $	��320��6$   $	��240��60   $��Bottom=�6=   $
��Left=�6X   6v   �� 5   
   -   5        $��	textblock��5      $
��Wrap=�6%   $	��150��69   $��40��6E   $	��Top=�6Q   $��Right=�6i   $��	0,20,80,0q�6�   $��	Distance:��6�   �� 6�   -   5        $��distanceData��5   �   $
��Wrap=�6�   $�� ��6�   6  6   �� 5   
   -   5        $��coordinates��5      $1��+Coordinates of depth pixel in color image: ��6'   $��40��6Z   $	��300��6f   $��Right=�6r   $��
0,0,10,600q�6�   �� 6�   -	   5        $��coordinatesText��5   �   $�� ��6�   6�   6�   �� 5   
   $��Right=�6   $��Center=�60   $��40��6K   $	��200��6W   $��Color pixel coordinates: ��6c   $��
0,0,80,500q�6�   �� 6�   -
   5        $��colorPixelCoordinates��5   �   $�� ��6�   6�   6�   �� 5   
   -   5        $��50��5   E   $��Toggle Valid Color Pixels��6Q   $��Right=�6u   $��Center=�6�   $��0,178,50,450q�6�   �� 5   
   -   5        $��50��5   F   $��Clear Previous Clicks��6R   $��Right=�6r   $��Center=�6�   $��	0,0,50,10q�6�   6�   5      5      
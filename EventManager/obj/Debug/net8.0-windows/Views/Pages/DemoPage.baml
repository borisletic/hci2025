   M S B A M L   `   `   `  ����  EventManager'    !EventManager.Views.Pages.DemoPage   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      .��5      �    LightGrayBrush#��[  6   .+�5   
   �   ActionBarStyle#��[ 6   $	��Top=�67   .+�5      ��   �� Orientation$  
Horizontal=�6   $��Center=�62   .+�5	      �� -   5         ��Name$ 
BackButton��5	        HamburgerButton#��[ 6-   $	����5
      6+   �� 5      $��	Demo Mode��6   $��18(�6-   $
��Bold"�6;     
WhiteBrush#��[ 5      $��Center=�6D   $
��10,0q�6_   5      5      �� 5   
   $��0,60,0,0q�6   $
��Auto=�5      $��Disabled=�5      .��5      ��   MobileContainer#��[ 6   $��20,20,20,20q�6B   .+�5      �� #��[ 6     PrimaryBlueBrush#��[ 5      $��2q�5       �� CornerRadius$ 10��5       �� Padding$ 20q�5      $
��0,20q�5      ._�5      �� .+�5       �� $��📺 Demo Mode��6$   $��24(�68   $
��Bold"�6F   $��Center=�6X   $��0,0,0,15q�6u   �� 5"      $;��5Watch the app demonstrate its features automatically!��6$   $��16(�5#   #   $
��Wrap=�5$   #   $��Center=�5%   #   $��0,0,0,20q�5&   #   �� 5(      -   5         ��Name$ DemoStatusText��5(   $   $��Demo is ready to start��5)   #   $��14(�5*   #   $��Italic$�5+   #   $��Center=�5,   #   $��0,0,0,20q�5-   #   $
��Wrap=�5.   #   �� 51      $  
Horizontal=�6%   $��Center=�6>   $��0,0,0,10q�6[   .+�52      �� -   5        $ StartDemoButton��52   %   $��▶️ Start Demo��53   $     SuccessButton#��[ 54   $   $��5q�56   $   �� 58      -   5        $ StopDemoButton��58   %   $��⏹️ Stop Demo��59   $     DangerButton#��[ 5:   $   $��5q�5<   $   ${�False��5=   $   5>      5?      5@      �� 5C      #��[ 6     DarkGrayBrush#��[ 5D      $��1q�5E      $ 8��5F      $ 15q�5G      $
��0,20q�5H      ._�5J      �� .+�5K      �� $��💡 Demo Features:��6$   $��16(�6=   $
��Bold"�6K   $��0,0,0,10q�6]   �� 5M      $.��(• Automatic navigation between screens��6$   $��14(�6R   $	��0,2q�6`   $
��Wrap=�6m   �� 5N      $3��-• Shows event list and search functionality��6$   $��14(�6W   $	��0,2q�6e   $
��Wrap=�6r   �� 5O      $)��#• Demonstrates world map features��6$   $��14(�6M   $	��0,2q�6[   $
��Wrap=�6h   �� 5P      $'��!• Simulates creating new events��6$   $��14(�6K   $	��0,2q�6Y   $
��Wrap=�6f   �� 5Q      $*��$• Cycles through all main features��6$   $��14(�6N   $	��0,2q�6\   $
��Wrap=�6i   �� 5S      $7��1Press any key or the Stop button to end the demo.��6$   $��12(�5T   #   $��Italic$�5U   #   $��0,15,0,0q�5V   #   $
��Wrap=�5W   #   5X      5Y      �� 5\      #��[  6    	 SuccessGreenBrush#��[	 5]      $��2q�5^      $ 8��5_      $ 15q�5`      $
��0,20q�5a      ._�5c      �� .+�5d      �� $��✨ Perfect for Learning!��6$   $��16(�6C   $
��Bold"�6Q   $��0,0,0,10q�6c   #��[	 5e   #   �� 5g      $7��1This demo mode is specially designed to help you:��6$   $��14(�6]   $��0,0,0,10q�6k   $
��Wrap=�6}   �� 5i      $?��9👀 See how each feature works without clicking anything��6$   $��14(�6c   $	��0,2q�6q   $
��Wrap=�6~   �� 5j      $)��#🎯 Learn the app at your own pace��6$   $��14(�6M   $	��0,2q�6[   $
��Wrap=�6h   �� 5k      $3��-🔄 Watch the demo as many times as you need��6$   $��14(�6W   $	��0,2q�6e   $
��Wrap=�6r   �� 5l      $-��'⏹️ Stop anytime by pressing any key��6$   $��14(�6O   $	��0,2q�6]   $
��Wrap=�6j   �� 5n      $6��0Just sit back and watch the app show you around!��6$   $��13(�5o   #   $��Italic$�5p   #   $��0,10,0,0q�5q   #   $
��Wrap=�5r   #   #��[ 5s   #   5t      5u      �� 5x      -   5        $��🏠 Back to Home��5x       
 PrimaryButton#��[
 5y      $��Center=�5{      $
��0,30q�5|      5~      5      5�      5�      
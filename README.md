# 简介
识别手机电脑中的麻将图片，可以起到记牌器的作用

# 使用方法
1.引入项目
<pre>
  <code>Install-Package ZedGraph -Version 5.1.7</code>
  <code>Install-Package Emgu.CV -Version 4.5.5.4823</code>
</pre>
2.修改麻将图片模板
<pre>
  <code>
  pb1tong.ImageLocation = "show/1tong.png";
  pb2tong.ImageLocation = "show/2tong.png";
  pb3tong.ImageLocation = "show/3tong.png";
  pb4tong.ImageLocation = "show/4tong.png";
  pb5tong.ImageLocation = "show/5tong.png";
  pb6tong.ImageLocation = "show/6tong.png";
  pb7tong.ImageLocation = "show/7tong.png";
  pb8tong.ImageLocation = "show/8tong.png";
  pb9tong.ImageLocation = "show/9tong.png";
  pb1tiao.ImageLocation = "show/1tiao.png";
  pb2tiao.ImageLocation = "show/2tiao.png";
  pb3tiao.ImageLocation = "show/3tiao.png";
  pb4tiao.ImageLocation = "show/4tiao.png";
  pb5tiao.ImageLocation = "show/5tiao.png";
  pb6tiao.ImageLocation = "show/6tiao.png";
  pb7tiao.ImageLocation = "show/7tiao.png";
  pb8tiao.ImageLocation = "show/8tiao.png";
  pb9tiao.ImageLocation = "show/9tiao.png";
  pbDong.ImageLocation = "show/dong.png";
  pbNan.ImageLocation = "show/nan.png";
  pbXi.ImageLocation = "show/xi.png";
  pbBei.ImageLocation = "show/bei.png";
  pbZhong.ImageLocation = "show/zhong.png";
  pbFa.ImageLocation = "show/fa.png";
  pbBa.ImageLocation = "show/ba.png";
  </code>
</pre>
3.将麻将图片模板复制到image下的每个文件夹，wojun是自己手上的牌，dong、nan、xi、bei是桌面上打出去的牌

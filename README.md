<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
# ParticleEffectProfiler
Unity 特效性能分析工具


![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E5%9B%BE%E7%89%8720190823003254.png)

简介：
---
该分析工具，主要是把特效的几个重要指标：内存、DrawCall、粒子数量、overdraw等数据显示在scene上，方便美术直接查看，并根据相关内容作出优化。  
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190910215020.png)

使用：
---
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126165417.png)  
将场景中的相机对准好特效，添加要测试的特效（只能一个），右键--特效--测试，（会自动添加ParticleEffectScript脚本，并运行Unity）  

ParticleEffectScript脚本：
---
会记录Overdraw、DrawCall、粒子数量这三个数据，并可以以折线图的形式展示。  
折线图内一个点代表一帧，如果没有勾选循环，则默认会记录3秒的数据，一秒30帧，会记录90个点。  
可以修改特效运行时间而修改打点的长度。
如果是循环特效，就勾选循环，数据就会不断记录。

![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126174343.png)  

无法自动裁剪（automic culling）：
---
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E5%9B%BE%E7%89%8720190126171957.png)  
由于特效的制作过程中，会使用一些非线性的运算，使得Unity无法实时获取到粒子的位置，会导致超出摄像机的粒子无法进行自动裁剪，所以在面板统一显示方便查看。  
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126174337.png)  
但这里的还不够全面，有部分无法自动裁剪的没法判断，因为有部分属性无法访问，有兴趣的可以反编译UnityEditor.dll，搜索UpdateCullingSupportedString这个函数进行了解。  

摄像机：
---
进行测试特效的时候，会修改摄像机的shader， 显示成类似Scene视图的Overdraw模式。  

支持：
---
Unity5.x 和 Unity2017.x

License
---
This library is under the MIT License.

感谢 [狂飙](https://github.com/networm) 的[建议](https://networm.me/2019/07/28/unity-particle-effect-profiler/)

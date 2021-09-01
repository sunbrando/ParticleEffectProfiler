<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>
# ParticleEffectProfiler
Unity 特效性能分析工具（[来自UWA公众号的安利](https://mp.weixin.qq.com/s/bKUwKw6VeJzk8fqIyKYjVQ)）  
2020-8-26：很荣幸此工具已被UWA集成，有购买UWA服务的在UWA里使用效果会更舒适（[UWA已支持批量检测粒子特效性能](https://mp.weixin.qq.com/s/Bdb3O8O6Ve3Zav9oQEiVjA)）

![inspector](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/jdfw%20(1).gif)

简介：
---
该分析工具，主要是把特效的几个重要指标：内存、DrawCall、粒子数量、overdraw等数据显示在scene上，方便美术直接查看，并根据相关内容作出优化。  
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190910215020.png)

使用：
---
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126165417.png)  
将场景中的相机对准好特效，选中要测试的特效（只能一个），右键--特效--测试，即会自动运行Unity，开始特效测试。（会自动添加ParticleEffectScript脚本，停止运行时脚本会自动删除，避免造成脚本污染）  

ParticleEffectScript脚本：
---
会记录Overdraw、DrawCall、粒子数量这三个数据，并可以以折线图的形式展示。（建议的数值请根据自己项目情况进行修改）  
折线图内一个点代表一帧，如果没有勾选循环，则默认会记录3秒的数据，一秒30帧，会记录90个点。  
可以修改特效运行时间而修改打点的长度。
如果是循环特效，请暂停后，勾选循环，数据就会不断记录。

![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126174343.png)  

无法自动剔除（automic culling）：
---
![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E5%9B%BE%E7%89%8720190126171957.png)  
当特效出现这个提示时候，自动剔除会关闭。
一般Unity在特效超出屏幕范围的时候，就会剔除此特效，但由于特效的制作过程中，会使用一些非线性的运算，导致Unity无法实时获取到粒子的位置，这就导致即使特效超出屏幕了Unity也会继续更新，无法对其自动剔除。[(Unity的文档说明)](https://docs.unity3d.com/ScriptReference/ParticleSystem-automaticCullingEnabled.html)

![Image text](https://github.com/sunbrando/ParticleEffectProfiler/blob/master/Document/QQ%E6%88%AA%E5%9B%BE20190126174337.png) 

当游戏中特效多的时候这也是很影响性能的一个点，所以在面板统一显示方便查看。

但这里的还不够全面，有部分自动裁剪的无法判断，因为有部分属性无法访问，有兴趣的可以反编译UnityEditor.dll，搜索UpdateCullingSupportedString这个函数进行了解，希望有想到其他解决办法的小伙伴可以在这里贡献下代码。  

摄像机：
---
进行测试特效的时候，会修改摄像机的shader， 显示成类似Scene视图的Overdraw模式。
特效的位置最好是正好全覆盖屏幕，不要太近，也不要太远（如果启用了[Mipmap](https://learnopengl-cn.github.io/01%20Getting%20started/06%20Textures/#_4)，太远有效像素会变少，导致平均Overdraw也少计算）。

支持：
---
Unity5.x
Unity2017.x
Unity2019.x

License
---
This library is under the MIT License.

感谢 [狂飙](https://github.com/networm) 的[建议](https://networm.me/2019/07/28/unity-particle-effect-profiler/)

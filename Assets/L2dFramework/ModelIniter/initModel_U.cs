using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;//引入命名空间
using System;

public class initModel_U : MonoBehaviour
{
    /*模型的操作需要指定深度2,宽度3,高度3.*/
    //模型内置参数
    public string Name;
        //模型相对位置
    public float MposX;
    public float MposY;

    //鼠标跟踪输入
    private L2DTargetPoint drag;

    /*-*/
    //模型外部输入
    public int modelAnimationFadeOut = 2000;//模型渲染后置延时
    public int modelAnimationFadein = 1000;//模型渲染前置延时
    //模型输入
    public TextAsset modelFile;//模型文件
    public Texture2D[] textures;//贴图文件
    private Live2DModelUnity live2DModel;//模型
    private Matrix4x4 live2DCanvansPos;//模型绘制矩阵
    //动作输入
    public TextAsset[] motionFile;//动作文件
    private Live2DMotion[] motions;//动作文件
    private MotionQueueManager motionQueueManager;//动作队列
    public int motionIndex;//当前动作位置
    //键位设定
    public KeyCode MTN_UP = KeyCode.W;//表情上翻
    public KeyCode MTN_RESET = KeyCode.Q;//表情重置
    public KeyCode MTN_DW = KeyCode.S;//表情下翻
    public KeyCode QUIT = KeyCode.Escape;//退出程序
    public KeyCode EyeballTraceMoveMent = KeyCode.E;//眼球移动
    //实体参数
    public Boolean flag_Eyeball = true;//眼球是否跟随
    public float ddx;
    public float ddy;

    void Start()
    {
        initModel();//初始化模型
        initialize_seq();
    }
    //程序入口点
    private void initModel()
    {
        Live2D.init();
        //初始化
        //Live2DModelUnity.loadModel(Application.dataPath + /*root*/"/Md/AyaX/aya_live_sr01_t03.moc");
        live2DModel = Live2DModelUnity.loadModel(modelFile.bytes);
        Texture2D texture2D = Resources.Load<Texture2D>("");
        //载入模型
        live2DModel.setTexture(0, texture2D);
        for (int i = 0; i < textures.Length; i++) { live2DModel.setTexture(i, textures[i]); }
        //循环取值贴图
        float modelWidth = live2DModel.getCanvasWidth();
        live2DCanvansPos = Matrix4x4.Ortho(0, modelWidth, modelWidth, 0, -50, 50);
        //加载贴图材质
        //live2DMotionIdle = Live2DMotion.loadMotion(Application.dataPath + "/Md/AyaX/idle01.mtn");
        //TextAsset mtnFile = Resources.Load<TextAsset>("");
        //live2DMotionIdle = Live2DMotion.loadMotion(mtnFile.bytes);
        //指定默认动作
        motions = new Live2DMotion[motionFile.Length];
        for (int i = 0; i < motions.Length; i++) { motions[i] = Live2DMotion.loadMotion(motionFile[i].bytes); }
        //循环取值动作文件
        motions[0].setLoopFadeIn(true);
        motions[0].setFadeOut(2000);
        motions[0].setFadeIn(2000);
        motions[0].setLoop(true);
        //默认动作行为
        motionQueueManager = new MotionQueueManager();
        //新建动作队列
        motionQueueManager.startMotion(motions[0]);
        //加载动作队列第一个
        drag = new L2DTargetPoint();
        //定义拖拽
    }
    /*模型初始化+载入贴图+指定动作+定义拖拽*/
    private void initialize_seq()
    {
        this.Name = this.name;
        //获取实例名称
        this.MposX = this.transform.position.x + Screen.width;
        this.MposY = this.transform.position.y + Screen.height;
        //获取实例位置(桌面画布坐标系)

    }
    /*模型参数校正*/
    void Update()
    {
        if (Input.GetKeyDown(QUIT))
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        //退出程序监听()
        live2DModel.setMatrix(transform.localToWorldMatrix * live2DCanvansPos);
        //转换矩阵
        Motion();
        //动作实现
        DragMove();
        live2DModel.update();
        //更新画板
    }
    //全局监听(步骤函数)
    private void Motion()
    {
        /*******表情更新*******/
        if (Input.GetKeyDown(MTN_DW))
        {
            motionIndex++;
            if (motionIndex >= motions.Length) { motionIndex = 0; }//出顶反底
            else if (motionIndex < 0) { motionIndex = motions.Length - 1; }//触底反弹
            motionQueueManager.startMotion(motions[motionIndex]);//队列更新
        }//表情下翻
        if (Input.GetKeyDown(MTN_RESET))
        {
            motionIndex = 0;
            motionQueueManager.startMotion(motions[motionIndex]);
        }//表情重置
        if (Input.GetKeyDown(MTN_UP))
        {
            motionIndex--;
            if (motionIndex >= motions.Length) { motionIndex = 0; }//出顶反底
            else if (motionIndex < 0) { motionIndex = motions.Length - 1; }//触底反弹
            motionQueueManager.startMotion(motions[motionIndex]);//队列更新
        }//表情上翻
        if (Input.GetKeyDown(EyeballTraceMoveMent)) 
        { 
            this.flag_Eyeball = !flag_Eyeball; 
        }
        //眼球跟踪
        /*-*/
        motions[motionIndex].setLoopFadeIn(true);
        motions[motionIndex].setFadeOut(modelAnimationFadeOut);
        motions[motionIndex].setFadeIn(modelAnimationFadein);
        motions[motionIndex].setLoop(true);
        //以上动画的更换方式(*过渡)
        motionQueueManager.updateParam(live2DModel);
        //更新队列
        /*******表情更新*******/
    }
    /*动作实现类*/
    private void DragMove()
    {
        Vector3 pos = Input.mousePosition;
        /*--pos.x|pox.y--*/

        this.ddx = pos.x / (Screen.width) * 2 - 1;//绝对坐标变换
        this.ddy = pos.y / (Screen.height) * 2 - 1;//绝对坐标变换
        float dx;//真实轴向坐标
        float dy;//真实轴向坐标
        /*-*/
        //if (pos.x > TrueX) { dx = -ddx; } else { dx = ddx; }
        //if (pos.y > TrueY) { dy = -ddy; } else { dy = ddy; }
        /*-*/
        if (Input.GetMouseButton(0)) { drag.Set(this.ddx,this.ddy); }
        //把屏幕坐标转换成live2D检测的坐标
        else if (Input.GetMouseButtonUp(0)) { drag.Set(0, 0); }
        //点击重设位置
        //参数及时更新，考虑加速度等自然因素,计算坐标，进行逐帧更新
        drag.update();
        //模型的转向
        if (drag.getX() != 0)
        {
            live2DModel.setParamFloat("PARAM_ANGLE_X", 30 * drag.getX());
            live2DModel.setParamFloat("PARAM_ANGLE_Y", 30 * drag.getY());
            live2DModel.setParamFloat("PARAM_BODY_ANGLE_X", 10 * drag.getX());
            if (flag_Eyeball == true)
            {
                //眼睛跟随鼠标移动
                live2DModel.setParamFloat("PARAM_EYE_BALL_X", drag.getX());
                live2DModel.setParamFloat("PARAM_EYE_BALL_Y", drag.getY());
            }
            else
            {
                //眼睛只望向前方
                live2DModel.setParamFloat("PARAM_EYE_BALL_X", -drag.getX());
                live2DModel.setParamFloat("PARAM_EYE_BALL_Y", -drag.getY());
            }
        }
    }
    /*跟随鼠标移动实现类*/


    /**********/
    /*-*/
    private void OnRenderObject()
    {
        live2DModel.draw();
        //绘图方法
    }
    /*-*/
}
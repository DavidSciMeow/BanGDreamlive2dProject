using UnityEngine;
using System.Collections;
 
public class WindowsBehave : MonoBehaviour {
    public Rect windowDialog=new Rect(20,20,150,100);
    private bool isOnDraw=true;
    void Start () {

    }
    void Update () {

    }

    void OnGUI(){
        if(isOnDraw){
            //id,pos,callback,name
        windowDialog=GUI.Window(0,windowDialog,doMyWindow,"这是一个可拖动的窗口");
        }
    }
    //创建一个可拖动窗口 的方法 
    void doMyWindow(int windowId){
        if(GUI.Button(new Rect(10, 20, 100, 20), "关闭窗口")){
            isOnDraw=false;
        }
    //使用DragWindow设置window窗体为可被鼠标拖动移动，并设置window窗体的鼠标响应范围，四个值分别是窗体中响应区的开始X、Y位置（窗体中的局部坐标），响应区的长宽。
    //GUI.DragWindow(new Rect(0, 0, 150,20)); 
    GUI.DragWindow();
    }
}

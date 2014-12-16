using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    public bool setAgentWalkMask;//是否需要动态修改寻路层，在scene4的实例中要用到

    void Start()
    {
        //获取寻路组件
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        //鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            //摄像机到点击位置的的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //判断点击的是否地形
                if (!hit.collider.tag.Equals("Plane"))
                {
                    return;
                }
                //点击位置坐标
                Vector3 point = hit.point;
                //转向
                transform.LookAt(new Vector3(point.x, transform.position.y, point.z));
                //设置寻路的目标点
                agent.SetDestination(point);
            }
        }

        //播放动画
        if (agent.remainingDistance == 0)
        {
            animation.Play("Idle");
        }
        else
        {
            animation.Play("Run");
        }
        
    }

    //动态设置寻路路径层
    void OnGUI()
    {
        if (!setAgentWalkMask)
        {
            return;
        }
        if (GUI.Button(new Rect(0, 0, 100, 50), "走下层"))
        {
            agent.walkableMask = 65;
        }

        if (GUI.Button(new Rect(0, 100, 100, 50), "走上层"))
        {
            agent.walkableMask = 129;
        }
    }
}

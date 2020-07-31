using UnityEngine;
using MLAgents;
using MLAgents.Sensors;
using MLAgents.SideChannels;

public class CartPoleAgent : Agent
{
    [Header("Specific to Pole")]
    public GameObject pole;
    Rigidbody poleRB;
    float angle_z;
    float cart_x;
    float anguVelo_z;
    FloatPropertiesChannel m_ResetParams;

    public override void Initialize()
    {
        poleRB = pole.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.FloatProperties;
        SetResetParameters();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(cart_x);
        sensor.AddObservation(angle_z);
        sensor.AddObservation(anguVelo_z);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Debug.Log(vectorAction[0].ToString());

        //行動
        MoveAgent(vectorAction);

        //カートの位置、ポールの角度と角速度
        cart_x = transform.localPosition.x;
        angle_z = pole.transform.localRotation.eulerAngles.z;
        //angle_zを-180~180度に変換
        if (180f < angle_z && angle_z < 360f)
        {
            angle_z = angle_z - 360f;
        }
        anguVelo_z = poleRB.angularVelocity.z;

        //状態の遷移
        StateTransition();
    }

    public void MoveAgent(float[] act)
    {
        //0-Left, 1-Right
        //int action = Mathf.FloorToInt(act[0]);
        //
        //if (action == 0)
        //{
        //    transform.Translate(-0.05f, 0, 0);
        //}
        //if (action == 1)
        //{
        //    transform.Translate(0.05f, 0, 0);
        //}

        transform.Translate(0.1f * act[0], 0, 0);
    }

    public void StateTransition()
    {

        //ポールの角度が-30~30度の時、報酬+0.01、それ以上傾いたら報酬-1
        if (-30f < angle_z && angle_z < 30f)
        {
            AddReward(0.01f);
        }
        if ((-180f < angle_z && angle_z < -30f) || (30f < angle_z && angle_z < 180f))
        {
            AddReward(-1f);
            EndEpisode();
        }

        //カートの位置が-3~3の範囲を越えたら報酬-1
        if (cart_x < -3f || 3f < cart_x)
        {
            AddReward(-0.01f);
            //AddReward(-1f);
            //EndEpisode();
        }
    }

    public override void OnEpisodeBegin()
    {
        //エージェントの状態をリセット
        transform.localPosition = new Vector3(0f, 0f, 0f);
        pole.transform.localPosition = new Vector3(0f, 1f, 0f);
        pole.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        poleRB.velocity = new Vector3(0f, 0f, 0f);
        //ポールにランダムな傾きを与える
        poleRB.angularVelocity = new Vector3(0f, 0f, Random.Range(-0.5f, 0.5f));

        //Reset the parameters when the Agent is reset.
        SetResetParameters();
    }
    
    public override float[] Heuristic()
    {
        var action = new float[1];

        action[0] = Input.GetAxis("Horizontal");

        return action;
    }

    public void SetPole()
    {
        //Set the attributes of the ball by fetching the information from the academy
        poleRB.mass = m_ResetParams.GetPropertyWithDefault("mass", 1.0f);
        //var scale = m_ResetParams.GetPropertyWithDefault("scale", 1.0f);
        //pole.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetResetParameters()
    {
        SetPole();
    }
}

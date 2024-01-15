using System.Collections.Generic;
using UnityEngine;

public class DroneInput : MonoBehaviour
{
    //Поля, отвечающие за восприимчивость к сигналам с осей джойстика
    [SerializeField] private bool thrustEnable = true;
    [SerializeField] private bool yawEnable = true;
    [SerializeField] private bool pitchEnable = true;
    [SerializeField] private bool rollEnable = true;

    //Текущие значения, полученные с осей, а также ложные значения, активирующиеся при поломке пропеллеров
    private float thrust,brokenThrust;
    private float yaw, brokenYaw;
    private float pitch, brokenPitch;
    private float roll, brokenRoll;
    private List<int> propellers = new List<int>();

    //Публичные свойства для получения значений с осей джойстика
    public float GetThrust() { return thrust; }
    public float GetYaw() { return yaw; }
    public float GetPitch() { return pitch; }
    public float GetRoll() { return roll; }

    //Свойство, возвращающее список с номерами сломанных пропеллеров
    public List<int> GetBrokens() { return propellers; }

    //Метод определения сломанного пропеллера и изменения показаний ложных значений с осей при поломке
    public void BrokeDrone(List<Propellers> brokenPropellers)
    {
        propellers.Clear();
        for(var i = 0; i<4; i++)
        {
            if(brokenPropellers.Contains((Propellers)i) && !propellers.Contains(i))
            {
                propellers.Add(i);
            }
        }
        brokenPitch = 0;
        brokenRoll = 0;
        brokenThrust = 0;
        brokenYaw = 0;
        if (brokenPropellers.Contains(Propellers.LeftFront) || brokenPropellers.Contains(Propellers.RightFront))
        {
            brokenPitch += 2f;
        }
        if (brokenPropellers.Contains(Propellers.LeftBack) || brokenPropellers.Contains(Propellers.RightBack))
        {
            brokenPitch -= 2f;
        }
        if (brokenPropellers.Contains(Propellers.LeftFront) || brokenPropellers.Contains(Propellers.LeftBack))
        {
            brokenRoll -= 1.1f;
        }
        if (brokenPropellers.Contains(Propellers.RightFront) || brokenPropellers.Contains(Propellers.RightBack))
        {
            brokenRoll += 1.1f;
        }
        if (brokenPropellers.Contains(Propellers.LeftFront) || brokenPropellers.Contains(Propellers.RightBack))
        {
            brokenYaw += 0.3f;
        }
        if (brokenPropellers.Contains(Propellers.RightFront) || brokenPropellers.Contains(Propellers.LeftBack))
        {
            brokenYaw -= 0.3f;
        }
        brokenThrust -= brokenPropellers.Count+ brokenPropellers.Count*0.5f;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        yaw = yawEnable ? Input.GetAxis("j4")+brokenYaw:0;
        thrust = thrustEnable ? Input.GetAxis("j3")+brokenThrust : 0;
        roll = rollEnable ? - Input.GetAxis("Horizontal")+brokenRoll : 0;
        pitch = pitchEnable ? - Input.GetAxis("Vertical")+brokenPitch : 0;
#else
        yaw= yawEnable ? Input.GetAxis("j3")+brokenYaw:0;
        thrust = thrustEnable ? Input.GetAxis("j14")+brokenThrust:0;
        roll = rollEnable ? -Input.GetAxis("Horizontal")+brokenRoll:0;
        pitch= pitchEnable ? -Input.GetAxis("Vertical")+brokenPitch:0;
#endif
    }
}

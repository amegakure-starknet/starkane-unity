using System;
using System.Linq;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;
using UnityEngine.UI;

public class Position : ModelInstance
{
    // model fields
    public dojo.FieldElement player;
    // public Vector2Int position => new Vector2Int((int)x, (int)y);
    public UInt32 x = 0;
    public UInt32 y = 0;


    // component fields
    public TextMesh textTag;
    public string shortPlayerAddress;

    public override void Initialize(Model model)
    {
        player = model.members["player"].ty.ty_primitive.contract_address;
        // x = model.members["vec"].ty.ty_struct.children[0].ty.ty_primitive.u32;
        SetXPosition(model.members["vec"].ty.ty_struct.children[0].ty.ty_primitive.u32);
        y = model.members["vec"].ty.ty_struct.children[1].ty.ty_primitive.u32;
    }

    void Start()
    {
        var target = new Vector3(x, 1, y);
        gameObject.transform.position = target;

        // convert bytes array to hex string
        var hexString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        var shortString = hexString.Substring(0, 8).TrimStart('0');

        shortPlayerAddress = $"0x{shortString}";
    }

    void Update()
    {
        // our curent position is gameObject.transform.position
        // move towards the target position
        var step = 3.0f * Time.deltaTime;
        // scale down our positions
        Vector3 oldPosition = gameObject.transform.position;
        var target = new Vector3(x, oldPosition.y, y);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, step);

        // calculate and display velocity
        Vector3 velocity = (gameObject.transform.position - oldPosition) / Time.deltaTime;
        if (textTag != null)
            textTag.text = $"{shortPlayerAddress}\nVelocity: {velocity.magnitude}";

        // if we are close enough to the target position, snap to it
        if (Vector3.Distance(gameObject.transform.position, target) < 0.001f)
        {
            gameObject.transform.position = target;
        }
    }

    public override void OnUpdated(Model model)
    {
        base.OnUpdated(model);
    }

    public void SetXPosition(UInt32 value)
    {
        if (value != x)
        {
            Debug.Log("Changed X! old value: " + x + " new value: " + value );
            x = value;
        }
    }
}
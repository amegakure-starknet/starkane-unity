using System;
using Dojo;
using Dojo.Torii;
using dojo_bindings;
using UnityEngine;

public class Ranking : ModelInstance
{
    private UInt32 id;
    private dojo.FieldElement player;
    private int playerId;
    private UInt64 score;

    public override void Initialize(Model model)
    {
        id = model.members["id"].ty.ty_primitive.u32;
        player = model.members["player"].ty.ty_primitive.felt252;
        score = model.members["score"].ty.ty_primitive.u64;

        var playerString = BitConverter.ToString(player.data.ToArray()).Replace("-", "").ToLower();
        // Assuming the 'player' field is a hexadecimal representation of felt252 data
        // Convert it to an integer or process it as needed
        playerId = System.Int32.Parse(playerString, System.Globalization.NumberStyles.AllowHexSpecifier);

        Debug.Log("Ranking: \n id: " + id
                  + "\n playerId: " + playerId
                  + "\n score: " + score + "\n");
    }
}

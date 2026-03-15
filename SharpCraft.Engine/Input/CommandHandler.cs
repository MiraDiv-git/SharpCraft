namespace SharpCraft.Engine.Input;

public static class CommandHandler
{
    public static void Execute(string command)
    {
        Console.WriteLine($"Command executed: {command}");
    }
    
    // $ALL - All players
    // $SELF - This player
    // $NEAREST - Nearest player
    // $ENTITY - All entities 
    // $RANDOM - Random player
    //
    // $ENTITY{Type}
    //
    // . - current player position
    // .+10 - current + 10 block
    // .-10 - current - 10 blocks
    //
    // block [action] {coordinates}
    // block place 12 3 6
    // block destroy 12 3 6
    // block remove 12 3 6
    //
    // teleport [who] [where]
    // teleport $SELF 12 4 6
    // teleport Player1 Player2
    // teleport $ALL Player1
    //
    // var [action] [name] {value}
    // var create MyVariable 3
    // var print MyVariable
    // var set MyVariable 0
    // var remove MyVariable 
    //
    // item give {item name} {count} (attributes)
    // item give dirt_block
    // item give grass_block 3 {Name: "Cool block"}
    //
    // clear [what] {where}
    // clear chat
    // clear chat global
    // clear inventory Player1
    // clear blocks from 12 3 6 to 15 8 12
    //
    // fill {where} [block]
    // fill from 12 3 6 to 15 8 12 grass_block
    //
    // sendpos [who]
    // sendpos $ALL
    // sendpos Player2
    //
    // checkpoint (who) (position)
    // checkpoint 
    // checkpoint Player1 12 3 6
}
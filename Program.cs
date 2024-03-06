using NitricEngine2D;

if (args.Contains("editMode"))
{
    GameManager.SetEditMode();
}
GameManager.NODE_NAMESPACES.Add("NitricEngine2D.nodes");
GameManager.NODE_NAMESPACES.Add("NitricEngine2D.game_nodes");

GameManager.SetScene("scenes/particleTest.json");

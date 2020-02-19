using UnityEngine;


namespace PVCollider
{
    public static class UI
    {
        private static readonly string[] axes =
        {
            "X",
            "Y",
            "Z"
        };

        private static float xOffset = 0.0f;
        private static float yOffset = 0.0f;
        private static float zOffset = 0.0f;

        private const int uiWidth = 425;
        private const int uiHeight = 179;

        private static Rect window = new Rect(Screen.width / 2 - uiWidth / 2, 10, uiWidth, uiHeight);

        public static void InitDraggersUI()
        {
            xOffset = PVCollider._lookAtTargetOffset_x.Value;
            yOffset = PVCollider._lookAtTargetOffset_y.Value;
            zOffset = PVCollider._lookAtTargetOffset_z.Value;
        }

        public static void DrawOffsetsUI()
        {
            window = GUILayout.Window(982116323, window, DrawWindow, "Offsets UI", GUILayout.Width(uiWidth), GUILayout.Height(uiHeight));
        }

        private static void ChangeOffsetX(float offset)
        {
            PVCollider._lookAtTargetOffset_x.Value = offset;
        }

        private static void ChangeOffsetY(float offset)
        {
            PVCollider._lookAtTargetOffset_y.Value = offset;
        }

        private static void ChangeOffsetZ(float offset)
        {
            PVCollider._lookAtTargetOffset_z.Value = offset;
        }

        private static void DrawWindow(int id)
        {
            GUILayout.BeginHorizontal();

            GUILayout.BeginArea(new Rect(5, 20, uiWidth / 4f, uiHeight - 25), GUI.skin.box);

            GUILayout.BeginVertical();

            string xOffsetText = GUILayout.TextField(xOffset.ToString("0.00"));
            if (float.TryParse(xOffsetText, out xOffset))
            {
                ChangeOffsetX(xOffset);
            }

            string yOffsetText = GUILayout.TextField(yOffset.ToString("0.00"));
            if (float.TryParse(yOffsetText, out yOffset))
            {
                ChangeOffsetY(yOffset);
            }

            string zOffsetText = GUILayout.TextField(zOffset.ToString("0.00"));
            if (float.TryParse(zOffsetText, out zOffset))
            {
                ChangeOffsetZ(zOffset);
            }

            GUILayout.EndVertical();

            GUILayout.EndArea();       

            GUI.DragWindow();
        }
    }


}

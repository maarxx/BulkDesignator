using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BulkDesignator
{
    public class MainTabWindow_BulkDesignator : MainTabWindow
    {

        private const float BUTTON_HEIGHT = 50f;
        private const float BUTTON_SPACE = 10f;


        public MainTabWindow_BulkDesignator()
        {
            //base.forcePause = true;
        }

        public override Vector2 InitialSize
        {
            get
            {
                //return base.InitialSize;
                return new Vector2(250f, 400f);
            }
        }

        public override MainTabWindowAnchor Anchor =>
            MainTabWindowAnchor.Right;

        public override void DoWindowContents(Rect canvas)
        {
            base.DoWindowContents(canvas);

            for (int i = 0; i < 4; i++)
            {
                Rect nextButton = new Rect(canvas);
                nextButton.y = i * (BUTTON_HEIGHT + BUTTON_SPACE);
                nextButton.height = BUTTON_HEIGHT;

                string buttonLabel;
                switch (i)
                {
                    case 0:
                        buttonLabel = "Cancel All Hunting";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.Hunt))
                            {
                                d.Delete();
                            }
                        }
                        break;
                    case 1:
                        buttonLabel = "Cancel All Cut Plant";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.CutPlant))
                            {
                                d.Delete();
                            }
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.HarvestPlant))
                            {
                                d.Delete();
                            }
                        }
                        break;
                    case 2:
                        buttonLabel = "Current Colonist Shot Finished is: ";
                        /* TODO: Implement
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            component.colonistShotFinished = !curColonistShotFinished;
                        }
                        */
                        break;
                    case 3:
                        buttonLabel = "Current Enemy Shot Begun is: ";
                        /* TODO: Implement
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            component.enemyShotBegun = !curEnemyShotBegun;
                        }
                        */
                        break;
                }
            }
        }

    }
}

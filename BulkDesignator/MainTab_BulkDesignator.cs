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

            Text.Font = GameFont.Small;
            for (int i = 0; i <= 2; i++)
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
                            List<Designation> toDelete = new List<Designation>();
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.Hunt))
                            {
                                toDelete.Add(d);
                                //d.Delete();
                            }
                            foreach (Designation d in toDelete)
                            {
                                d.Delete();
                            }
                        }
                        break;
                    case 1:
                        buttonLabel = "Cancel All Cut/Harvest";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            List<Designation> toDelete = new List<Designation>();
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.CutPlant))
                            {
                                toDelete.Add(d);
                                //d.Delete();
                            }
                            foreach (Designation d in Find.VisibleMap.designationManager.DesignationsOfDef(DesignationDefOf.HarvestPlant))
                            {
                                toDelete.Add(d);
                                //d.Delete();
                            }
                            foreach (Designation d in toDelete)
                            {
                                d.Delete();
                            }
                        }
                        break;
                    case 2:
                        buttonLabel = "Operate All Mechanoids";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            foreach (Pawn p in Find.VisibleMap.mapPawns.AllPawnsSpawned)
                            {
                                if (p.ToString().Contains("Mechanoid") && p.Downed)
                                {
                                    foreach (RecipeDef recipe in p.def.AllRecipes)
                                    {
                                        if (recipe.AvailableNow)
                                        {
                                            IEnumerable<ThingDef> enumerable = recipe.PotentiallyMissingIngredients(null, p.Map);
                                            if (!enumerable.Any((ThingDef x) => x.isBodyPartOrImplant))
                                            {
                                                if (!enumerable.Any((ThingDef x) => x.IsDrug))
                                                {
                                                    if (recipe.targetsBodyPart)
                                                    {
                                                        foreach (BodyPartRecord bodyPart in recipe.Worker.GetPartsToApplyOn(p, recipe))
                                                        {
                                                            bool alreadyExists = false;
                                                            foreach (Bill b in p.BillStack.Bills)
                                                            {
                                                                if (b is Bill_Medical)
                                                                {
                                                                    Bill_Medical existing = (Bill_Medical)b;
                                                                    if (existing.recipe.label == recipe.label)
                                                                    {
                                                                        if (existing.Part == bodyPart)
                                                                        {
                                                                            alreadyExists = true;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            if (!alreadyExists)
                                                            {
                                                                Bill_Medical bm = new Bill_Medical(recipe);
                                                                p.BillStack.AddBill(bm);
                                                                bm.Part = bodyPart;
                                                                p.BillStack.Reorder(bm, 0);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        bool alreadyExists = false;
                                                        foreach (Bill b in p.BillStack.Bills)
                                                        {
                                                            if (b is Bill_Medical)
                                                            {
                                                                Bill_Medical existing = (Bill_Medical)b;
                                                                if (existing.recipe.label == recipe.label)
                                                                {
                                                                    alreadyExists = true;
                                                                }
                                                            }
                                                        }
                                                        if (!alreadyExists)
                                                        {
                                                            Bill_Medical bm = new Bill_Medical(recipe);
                                                            p.BillStack.AddBill(bm);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
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

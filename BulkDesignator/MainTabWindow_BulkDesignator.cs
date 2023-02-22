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

        public struct SurgeryOption
        {
            public string text;
            public RecipeDef recipe;
            public BodyPartRecord part;

            public bool equals(SurgeryOption so2)
            {
                return this.text == so2.text && this.recipe == so2.recipe && this.part == so2.part;
            }
        }

        //all from HealthCardUtility.DrawMedOperationsTab
        public string generateSurgeryText(Pawn pawn, RecipeDef recipe, BodyPartRecord part)
        {
            string text = recipe.Worker.GetLabelWhenUsedOn(pawn, part).CapitalizeFirst();
            if (part != null && !recipe.hideBodyPartNames)
            {
                text = text + " (" + part.Label + ")";
            }
            //text.Replace(" (teetotaler will be unhappy)", "");
            return text;
        }

        public bool doesPawnAlreadyHaveSurgery(Pawn pawn, SurgeryOption surgery)
        {
            foreach (Bill b in pawn.BillStack.Bills)
            {
                if (b is Bill_Medical)
                {
                    Bill_Medical existing = (Bill_Medical)b;
                    if (existing.recipe == surgery.recipe)
                    {
                        if (existing.Part == surgery.part)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool canPawnGetSurgery(Pawn pawn, SurgeryOption surgery)
        {
            foreach (RecipeDef allRecipe in pawn.def.AllRecipes)
            {
                if (allRecipe.AvailableNow && allRecipe.AvailableOnNow(pawn))
                {
                    IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
                    if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
                    {
                        if (allRecipe.targetsBodyPart)
                        {
                            foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                            {
                                SurgeryOption temp = new SurgeryOption();
                                temp.text = generateSurgeryText(pawn, allRecipe, item);
                                temp.recipe = allRecipe;
                                temp.part = item;
                                if (temp.equals(surgery)) { return true; }
                            }
                        }
                        else
                        {
                            SurgeryOption temp = new SurgeryOption();
                            temp.text = generateSurgeryText(pawn, allRecipe, null);
                            temp.recipe = allRecipe;
                            temp.part = null;
                            if (temp.equals(surgery)) { return true; }
                        }
                    }
                }
            }
            return false;
        }

        public List<SurgeryOption> getAllPossibleSurgeries(List<Pawn> pawns)
        {
            HashSet<SurgeryOption> set = new HashSet<SurgeryOption>();
            foreach (Pawn pawn in pawns)
            {
                foreach (RecipeDef allRecipe in pawn.def.AllRecipes)
                {
                    if (allRecipe.AvailableNow && allRecipe.AvailableOnNow(pawn))
                    {
                        IEnumerable<ThingDef> enumerable = allRecipe.PotentiallyMissingIngredients(null, pawn.Map);
                        if (!enumerable.Any((ThingDef x) => x.isTechHediff) && !enumerable.Any((ThingDef x) => x.IsDrug) && (!enumerable.Any() || !allRecipe.dontShowIfAnyIngredientMissing))
                        {
                            if (allRecipe.targetsBodyPart)
                            {
                                foreach (BodyPartRecord item in allRecipe.Worker.GetPartsToApplyOn(pawn, allRecipe))
                                {
                                    SurgeryOption temp = new SurgeryOption();
                                    temp.text = generateSurgeryText(pawn, allRecipe, item);
                                    temp.recipe = allRecipe;
                                    temp.part = item;
                                    if (!set.Contains(temp)) { set.Add(temp); }
                                }
                            }
                            else
                            {
                                SurgeryOption temp = new SurgeryOption();
                                temp.text = generateSurgeryText(pawn, allRecipe, null);
                                temp.recipe = allRecipe;
                                temp.part = null;
                                if (!set.Contains(temp)) { set.Add(temp); }
                            }
                        }
                    }
                }
            }
            return set.ToList();
        }

        public void addSurgeryIfNotAlready(List<Pawn> pawns, SurgeryOption surgery)
        {
            foreach (Pawn pawn in pawns)
            {
                if (canPawnGetSurgery(pawn, surgery) && !doesPawnAlreadyHaveSurgery(pawn, surgery))
                {
                    //Bill_Medical bm = new Bill_Medical(surgery.recipe);
                    //pawn.BillStack.AddBill(bm);
                    //bm.Part = surgery.part;
                }
            }
        }

        public override void DoWindowContents(Rect canvas)
        {
            //base.DoWindowContents(canvas);

            Text.Font = GameFont.Small;
            for (int i = 0; i <= 3; i++)
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
                            foreach (Designation d in Find.CurrentMap.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Hunt))
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
                            foreach (Designation d in Find.CurrentMap.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.CutPlant))
                            {
                                toDelete.Add(d);
                                //d.Delete();
                            }
                            foreach (Designation d in Find.CurrentMap.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.HarvestPlant))
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
                            List<Pawn> mechanoids = new List<Pawn>();
                            foreach (Pawn p in Find.CurrentMap.mapPawns.AllPawnsSpawned)
                            {
                                if (p.ToString().Contains("Mechanoid") && p.Downed)
                                {
                                    mechanoids.Add(p);
                                }
                            }
                            List<SurgeryOption> available = getAllPossibleSurgeries(mechanoids);
                            List<FloatMenuOption> menuAvailable = new List<FloatMenuOption>();
                            foreach (SurgeryOption so in available)
                            {
                                menuAvailable.Add(new FloatMenuOption(so.text, delegate { addSurgeryIfNotAlready(mechanoids, so); }));
                            }
                            if (menuAvailable.Count > 0)
                            {
                                Find.WindowStack.Add(new FloatMenu(menuAvailable));
                            }
                        }
                        break;
                    case 3:
                        buttonLabel = "Bulk Operate Humanoids";
                        if (Widgets.ButtonText(nextButton, buttonLabel))
                        {
                            List<Pawn> pawns = new List<Pawn>();
                            foreach (object obj in Find.Selector.SelectedObjects)
                            {
                                if (obj is Pawn)
                                {
                                    pawns.Add((Pawn)obj);
                                }
                            }
                            List<SurgeryOption> available = getAllPossibleSurgeries(pawns);
                            List<FloatMenuOption> menuAvailable = new List<FloatMenuOption>();
                            foreach (SurgeryOption so in available)
                            {
                                menuAvailable.Add(new FloatMenuOption(so.text, delegate { addSurgeryIfNotAlready(pawns, so); }));
                            }
                            if (menuAvailable.Count > 0)
                            {
                                Find.WindowStack.Add(new FloatMenu(menuAvailable));
                            }
                        }
                        break;
                }
            }
        }

    }
}

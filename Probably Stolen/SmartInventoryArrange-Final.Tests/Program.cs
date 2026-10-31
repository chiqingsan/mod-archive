using SmartInventoryArrange.Core;

var mask = new ShapeMask(new[] { new Cell(0,0) });
var fixedItem = new Placement("pinned", 1, 0, 0, mask, "~fixed", false);
var problem = new LayoutProblem {
    Width=3, Height=2,
    Usable=new bool[,] { {true,true}, {true,true}, {true,true} },
    Fixed=new[] {fixedItem},
    Items=new[] {
        new LayoutItem { StableId="item-a", ItemId="a", PrimaryCategory="item", Orientations=new[]{new Orientation(0,mask,false)}, OriginalMask=mask, OriginalX=0, OriginalY=1 },
        new LayoutItem { StableId="item-b", ItemId="b", PrimaryCategory="item", Orientations=new[]{new Orientation(0,mask,false)}, OriginalMask=mask, OriginalX=2, OriginalY=1 },
    }
};
var result = new DeterministicSolver().Solve(problem);
if (!result.Complete || result.Placements.Count!=2 || result.Placements.Any(p=>p.X==1 && p.Y==0) || result.Placements.Any(p=>p.StableId=="pinned"))
    throw new Exception("Solver moved or overlapped fixed item");
Console.WriteLine("PASS fixed position reserved while movable items are arranged");

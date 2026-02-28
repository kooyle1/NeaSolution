using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DevInputManager : MonoBehaviour
{
    [SerializeField] private DevUiManager uiManager;
    [SerializeField] private Solver solver;
    
    private void depthTest(List<(ulong, ulong)> positionList)
    {
        double totalTime = 0;
        int count = 0;

        int totalNodes = 0;

        foreach (var position in positionList) {
            count++;
            ulong pos = position.Item1;
            ulong board = position.Item2;
            solver.ReturnScores(pos, board);
            totalTime = solver.timeTaken;
            totalNodes += solver.exploredNodes;
        }

        uiManager.SetAverage(totalTime / count, totalNodes / count);
    }

    public void depth7Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (278921344, 34642870400), (22059220467712, 30889675325440), (44056576, 65028224), (287309826, 299892739), (4398314962945, 4432676798593), (4433748426752, 13299097796608),
            (4398046544512, 4398046561153), (272662656, 274776193), (4432406250112, 13228499272577), (10567680, 283230208), };
        depthTest(positionList);
    }

    public void depth9Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (34403794944, 34491875456), (4398052819200, 4398321287553), (4398323384320, 4432689414272), (34634498048, 4501408890880), (92358976733187, 277076930199683), (103622377472, 13298030362624), (68721574021, 103081329031),
            (4538438254592, 4917469118464), (22024592343040, 66005327986688), (815792256, 35196518528) };
        depthTest(positionList);
    }

    public void depth11Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)>{ (1923088384, 4434417418240), (35712434176, 4434300027008), (287424512, 34659746176), (13194145890432, 13194422763649),
                                                                    (819986560, 4501962506368), (268550657, 4432674931585), (48250881, 937443457), (34403827712, 34760343680),
                                                                    (43980475596805, 65970712346639), (3495985152, 4436447510528) };
        depthTest(positionList);
    }

    public void depth13Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (34940698624, 36370956416), (8796099379585, 13194414375811), (4468118781952, 4503019700224), (242944573440, 4642605940736),
            (815890688, 1910621056), (13194150068226, 13228782436739), (22024592294537, 30889673229199), (4776003633285, 31301721670023), (44090624, 132237184), (4467044941825, 4501945827457) };
        depthTest(positionList);
    }

    public void depth15Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (34940993536, 35297673344), (4536299208705, 4640458457219), (92358976733589, 277076930216383), (1080149248, 1893975937), (103358267776, 4501962736001), (4467581845760, 4640458523008),
            (172075565696, 242412012416), (92463131787392, 277181890576513), (35976724481, 4503019569537), (34902999680, 4501945959296) };
        depthTest(positionList);
    }

    public void depth17Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (37088477184, 38518898816), (344413274240, 519454310528), (105233072768, 4505167053697), (92393338585109, 277111560487103), (175292628993, 4642622783617), (36257693953, 4505183830401),
            (4879890317440, 13713576935552), (21992470315008, 30790484475904), (13195760697347, 13436551544963), (139362074752, 244676935808) };
        depthTest(positionList);
    }

    public void depth19Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (4435134988288, 4505284886656), (57664776192000, 67052775686144), (92359788462209, 277112101781891), (3500196102, 38417974151), (21992651259914, 65974727327759), (44325404688645, 66487974904711), (8867239280640, 13301310800000),
            (104476050048, 4505217516416), (22166607282176, 66219568840832), (481860616320, 14263349526656), };
        depthTest(positionList);
    }

    public void depth21Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (22096543563778, 136584018116611), (38191383808, 107238377344), (9798200360960, 31868453961728), (92358976817834, 277076930322367), (383906791424, 4921829343616), (8902437585152, 31031002513792), (4570117867141, 4640458524559),
            (22027283006336, 66215248775041), (22442009477248, 67044177395843), (104201404672, 4642656469889), };
        depthTest(positionList);
    }

    public void depth23Test()
    {
        List<(ulong, ulong)> positionList = new List<(ulong, ulong)> { (413149726336, 4917534640000), (93091807707136, 279258511556608), (5090360607232, 5471551604608), (233269610201089, 279245634977921), (74937533313, 4509579396999), (17695813174913, 30891316332417), (723437797516, 6564603872143),
            (4469763015808, 13301310803840), (4575268520065, 4921829344129), (4640720765187, 31030901982083) };
        depthTest(positionList);
    }

}

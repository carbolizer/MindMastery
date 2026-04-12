using TMPro;
using UnityEngine;

public class LabelMaker : MonoBehaviour
{
    public GameObject LabelPrefab;
    public Vector3 Offset = new Vector3(0.0359f, -0.0472f, 10);
    private bool init = false;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RouletteManager.Instance.RouletteTable != null && !init)
        {
            init = true;
            foreach (var item in RouletteManager.Instance.Cells)
            {
                var pref = Instantiate(LabelPrefab);

                pref.transform.position = new Vector3(item.transform.position.x - Offset.x, item.transform.position.y - Offset.y, -3);
                pref.GetComponent<LabelData>().DoesWin = item.GetComponent<RouletteCell>().doesWinNext;
                pref.GetComponent<TextMeshPro>().text = ((int)item.GetComponent<RouletteCell>().Cell).ToString();
                RouletteManager.Instance.labels.Add(pref);
            }

            foreach (var item in RouletteManager.Instance.ExtraCells)
            {
                var pref = Instantiate(LabelPrefab);
                var cellNum = (AssociatedNumberMode)(int)item.GetComponent<RouletteCell>().Cell - 37;
                pref.GetComponent<LabelData>().DoesWin = RouletteManager.Instance.RouletteTable[cellNum].m_wonNextRoll;
                RouletteManager.Instance.labels.Add(pref);

                
                switch (cellNum)
                {
                    case AssociatedNumberMode.Column1:
                    case AssociatedNumberMode.Column2:
                    case AssociatedNumberMode.Column3:
                    pref.transform.position = new Vector3(item.transform.position.x - Offset.x, item.transform.position.y - Offset.y, -3);
                    pref.GetComponent<TextMeshPro>().text = RouletteManager.Instance.RouletteTable[cellNum].m_name.Replace("Column", "");
                    break;

                    case AssociatedNumberMode.High:
                    pref.transform.position = new Vector3(item.transform.position.x - Offset.x, item.transform.position.y - Offset.y, -3);
                    pref.transform.eulerAngles = Vector3.zero;
                    pref.GetComponent<TextMeshPro>().text = "High";
                    break;

                    case AssociatedNumberMode.Low:
                    pref.transform.position = new Vector3(item.transform.position.x - Offset.x, item.transform.position.y - Offset.y, -3);
                    pref.transform.eulerAngles = Vector3.zero;
                    pref.GetComponent<TextMeshPro>().text = "Low";
                    break;

                    default:
                    pref.transform.position = new Vector3(item.transform.position.x - Offset.x, item.transform.position.y - Offset.y, -3);
                    pref.transform.eulerAngles = Vector3.zero;
                    pref.GetComponent<TextMeshPro>().text = RouletteManager.Instance.RouletteTable[cellNum].m_name;
                    break;

                }
            }
        }
    }
}

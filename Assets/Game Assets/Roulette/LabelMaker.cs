using TMPro;
using UnityEngine;

public class LabelMaker : MonoBehaviour
{
    public GameObject LabelPrefab;
    public Vector3 Offset = new Vector3(0.0359f, -0.0472f, -2);
    void Start()
    {
        foreach (var item in RouletteManager.Instance.Cells)
        {
            var pref = Instantiate(LabelPrefab);
            pref.transform.position = item.transform.position - Offset;
            pref.GetComponent<TextMeshPro>().text = ((int)item.GetComponent<RouletteCell>().Cell).ToString();
        }

        foreach (var item in RouletteManager.Instance.ExtraCells)
        {
            var pref = Instantiate(LabelPrefab);
            pref.transform.position = item.transform.position - Offset;
            pref.transform.eulerAngles = Vector3.zero;
            pref.GetComponent<TextMeshPro>().text = RouletteManager.Instance.RouletteTable[(AssociatedNumberMode)(int)item.GetComponent<RouletteCell>().Cell - 37].m_name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

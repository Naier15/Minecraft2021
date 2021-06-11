using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float jumpSpeed;
    public float speedy;
    public float gravity;
    private Vector3 moveDirection = Vector3.zero;

    private BlockType selectedBlock = BlockType.Grass;
    private UnityEngine.UI.Image grassIcon;
    private UnityEngine.UI.Image stoneIcon;
    private UnityEngine.UI.Image waterIcon;
    private UnityEngine.UI.Image dirtyIcon;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        grassIcon = GameObject.Find("GrassIcon").GetComponent<UnityEngine.UI.Image>();
        stoneIcon = GameObject.Find("StoneIcon").GetComponent<UnityEngine.UI.Image>();
        waterIcon = GameObject.Find("WaterIcon").GetComponent<UnityEngine.UI.Image>();
        dirtyIcon = GameObject.Find("DirtyIcon").GetComponent<UnityEngine.UI.Image>();

        SelectBlock(BlockType.Grass);
    }

	private void SelectBlock(BlockType block)
	{
        selectedBlock = block;
        grassIcon.GetComponent<RectTransform>().transform.localScale = 
            Vector3.one * (block == BlockType.Grass ? 1.2f : 1f);
        stoneIcon.GetComponent<RectTransform>().transform.localScale =
            Vector3.one * (block == BlockType.Stone ? 1.2f : 1f);
        waterIcon.GetComponent<RectTransform>().transform.localScale =
            Vector3.one * (block == BlockType.Water ? 1.2f : 1f);
        dirtyIcon.GetComponent<RectTransform>().transform.localScale =
            Vector3.one * (block == BlockType.Dirty ? 1.2f : 1f);
    }

    IEnumerator CheckWater(WorldGenerator wg, Vector3 pos)
    {
        var pos2 = pos; pos2.y--;
        while (true)
        {
            if (!Physics.Linecast(pos, pos2))
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(pos2, BlockType.Water);
            } else { break; }
            pos.y--;
            pos2.y--;
        }
    }

    IEnumerator CheckWater2(WorldGenerator wg, Vector3 pos)
    {
        pos.y--;
        var pos2 = pos; pos2.y--;
        while (true)
        {
            if (!Physics.Linecast(pos, pos2))
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(pos, BlockType.Water);
            }
            else {
                yield return new WaitForSeconds(1); 
                wg.CreateVoxel(pos, BlockType.Water); 
                break; 
            }
            pos.y--;
            pos2.y--;
        }
    }

    IEnumerator CheckWater3(WorldGenerator wg, Vector3 pos)
    {
        foreach (var elem in wg.water_blocks)
        {
            if(pos.x -2 == elem.x && pos.z == elem.z && pos.y == elem.y)
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(new Vector3(pos.x-1, pos.y, pos.z), BlockType.Water);
                StartCoroutine(CheckWater(wg, new Vector3(pos.x - 1, pos.y, pos.z)));
                break;
            }
            if (pos.x +2 == elem.x && pos.z == elem.z && pos.y == elem.y)
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(new Vector3(pos.x + 1, pos.y, pos.z), BlockType.Water);
                StartCoroutine(CheckWater(wg, new Vector3(pos.x + 1, pos.y, pos.z)));
                break;
            }
            if (pos.x == elem.x && pos.z -2 == elem.z && pos.y == elem.y)
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(new Vector3(pos.x, pos.y, pos.z-1), BlockType.Water);
                StartCoroutine(CheckWater(wg, new Vector3(pos.x, pos.y, pos.z - 1)));
                break;
            }
            if (pos.x == elem.x && pos.z +2 == elem.z && pos.y == elem.y)
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(new Vector3(pos.x, pos.y, pos.z+1), BlockType.Water);
                StartCoroutine(CheckWater(wg, new Vector3(pos.x, pos.y, pos.z + 1)));
                break;
            }
        }
    }

    public IEnumerator CheckWater4(WorldGenerator wg, Vector3 pos)
    {
        foreach (var elem in wg.water_blocks)
        {
            if ( (pos.x - 1 == elem.x && pos.z == elem.z && pos.y == elem.y) || (pos.x + 1 == elem.x && pos.z == elem.z && pos.y == elem.y) 
                || (pos.x == elem.x && pos.z - 1 == elem.z && pos.y == elem.y) || (pos.x == elem.x && pos.z + 1 == elem.z && pos.y == elem.y))
            {
                yield return new WaitForSeconds(1);
                wg.CreateVoxel(pos, BlockType.Water);
                break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection) * speedy;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        var lookX = Input.GetAxis("Mouse X");
        var lookY = Input.GetAxis("Mouse Y");


        transform.localEulerAngles += new Vector3(0, lookX, 0);
        var cube = transform.Find("Cube");
        if (cube != null)
        {
            cube.transform.localEulerAngles += new Vector3(-lookY, 0, 0);
        }


        var wg = GameObject.Find("WorldGenerator").GetComponent<WorldGenerator>();

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out var hit, 10))
            {
                if (hit.collider.name == "Cube" || hit.collider.name == "Water")
                {
                    Destroy(hit.collider.gameObject);
                    wg.water_blocks.Remove(hit.transform.position);
                    if (hit.collider.name == "Water")
                        StartCoroutine(CheckWater4(wg, hit.transform.position));

                    if (hit.collider.name == "Cube" || hit.collider.name == "Water")
                    { 
                        var pos = hit.collider.transform.position; pos.y++;
                        foreach (var elem in wg.water_blocks) { if (pos == elem) { StartCoroutine(CheckWater2(wg, pos)); } }
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out var hit, 10))
            {
                if (hit.collider.name == "Cube" || hit.collider.name == "Rock" || hit.collider.name == "Water")
                {
                    var pos = hit.transform.position + hit.normal;
                    wg.CreateVoxel(hit.transform.position + hit.normal, selectedBlock);

                    if (selectedBlock == BlockType.Water)
                    {
                        StartCoroutine(CheckWater(wg, pos));
                        StartCoroutine(CheckWater3(wg, pos));
                    }
                }
            }
        }
        

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectBlock(BlockType.Stone);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectBlock(BlockType.Grass);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectBlock(BlockType.Water);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectBlock(BlockType.Dirty);
        }
    }
}
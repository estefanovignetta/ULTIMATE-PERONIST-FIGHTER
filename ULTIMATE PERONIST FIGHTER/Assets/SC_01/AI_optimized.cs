using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AI_optimized : MonoBehaviour
{
    public GameObject player;
    public AudioClip _punch01, _punch02, _throw;
    public AudioClip[] taunt_sounds, punch_sounds, hit_sounds;

    public float speed;
    public int onAttack, attack_counter;
    private int onBeingHit;

    private BoxCollider bc, bc_theirs;
    private Vector3 target;
    private Animator anim;
    private Sound_Manager sm;

    public bool facing_right, facing_up;
    private Vector3 charscreenpos;
    public bool moving_right, moving_left, moving_up, moving_down;

    private float rnd;
    public List<ColliderRestrictions> MovementRestrictionList = new List<ColliderRestrictions>();

    public static int enemyCount;
    //public GameObject GameManager;
    private Game_Manager gm;

    private bool hasTarget;
    private Vector3 heading, direction;
    private float distance;

    public LayerMask obsavoidMask;
    private float targetCounter;

    // Use this for initialization
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = GameObject.Find("Game Manager").GetComponent<Game_Manager>();
        bc = GetComponent<BoxCollider>();
        bc_theirs = player.GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();
        sm = GetComponent<Sound_Manager>();

        onAttack = onBeingHit = 0;
        attack_counter = 0;
        facing_up = true;
        facing_right = false;
        moving_right = moving_left = moving_up = moving_down = true;

        enemyCount++;
        targetCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Walking", false);

        //CHECKING IF CHARACTER IS PUNCHING SOMETHING OR BEING PUNCHED
        if (onAttack > 0 || onBeingHit > 0) return;
        if (player == null) return;

        //MOVEMENT
        targetCounter++;
        if (targetCounter >= 30)
        {
            targetCounter = 0;
            hasTarget = false;
            //Debug.Break();
        }
        if (!hasTarget)
        {
            DefineTarget();
            ReDefineTarget();
            hasTarget = true;
        }
        direction = target - transform.position;
        direction = new Vector3(direction.x, direction.y, 0);
        distance = direction.magnitude;
        direction = direction.normalized;
        //heading = target - transform.position;
        //direction = heading / distance;
        //direction = new Vector3(heading.x, heading.y, 0).normalized;
        print("Distance es... " + distance + " y Direction es... " + direction);
        //Debug.Break();

        //CONSTRAINING HORIZONTAL MOVEMENT
        if ((direction.x > 0 && !moving_right) || (direction.x < 0 && !moving_left))
            direction.x = 0;

        //CONSTRAINING VERTICAL MOVEMENT
        if ((direction.y > 0 && !moving_up) || (direction.y < 0 && !moving_down))
            direction.y = 0;


        //EXECUTING MOVEMENT
        if ((Mathf.Abs(direction.x) > 0.0 || Mathf.Abs(direction.y) > 0.0))
        {
            transform.Translate(direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, direction.y * Time.deltaTime * speed);
            anim.SetBool("Walking", true);
            if ((direction.x > 0 && !facing_right) || (direction.x < 0 && facing_right))
                Flip();
        }
        if (distance < 0.1)
        {
            print("I reached my target!");
            //Debug.Break();
            targetCounter = 0;
            hasTarget = false;
        }

            /*
            //If far, I'll look towards my direction
            if (distance >= 2.0f)
            {
                if ((direction.x > 0 && !facing_right) || (direction.x < 0 && facing_right))
                    Flip();
            }
            //If close, I'll look at the player
            else
            {
                if ((facing_right && player.transform.position.x < transform.position.x) || (!facing_right && player.transform.position.x > transform.position.x))
                    Flip();
            }

            if (distance > 0.6 && (Mathf.Abs(direction.x) > 0.2 || Mathf.Abs(direction.y) > 0.2))
            {
                transform.Translate(direction.x * Time.deltaTime * speed, direction.y * Time.deltaTime * speed, direction.y * Time.deltaTime * speed);
                anim.SetBool("Walking", true);
                //Saying something when trying to engage
                if (Random.value > 0.99)
                    sm.SendMessage("PlayOutLoud", taunt_sounds[Random.Range(0, taunt_sounds.Length)]);
            }
            else if (distance <= 0.6)
            {
                 rnd = Random.value;
                 if (rnd <= 0.025 / 4)
                 {
                     anim.SetTrigger("Attack_01");
                     sm.SendMessage("PlayOutLoud", _throw);
                     sm.SendMessage("PlayOutLoud", punch_sounds[Random.Range(0, punch_sounds.Length)]);
                 }
                 else if (rnd <= 0.05 / 4 && rnd > 0.025 / 4)
                 {
                     anim.SetTrigger("Attack_02");
                     sm.SendMessage("PlayOutLoud", _throw);
                     sm.SendMessage("PlayOutLoud", punch_sounds[Random.Range(0, punch_sounds.Length)]);
                 }
                hasTarget = false;
                anim.SetBool("Walking", true);
            }
            */

        }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setOnAttack(int state)
    {
        onAttack = state;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setAttackCounter(int state)
    {
        attack_counter = state;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void setOnBeingHit(int state)
    {
        if (onBeingHit == 0 && state != 0) sm.SendMessage("PlayOutLoud", hit_sounds[Random.Range(0, hit_sounds.Length)]);
        onBeingHit = state;
        onAttack = 0;
        attack_counter = 0;
        return;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerEnter(Collider myCollider)
    {
        bool this_movingright, this_movingleft, this_movingup, this_movingdown;
        this_movingright = this_movingleft = this_movingup = this_movingdown = true;

        //CALCULATING THE VERTICES OF THE COLLIDERS
        float upper_ref = bc.center.y + bc.size.y / 2,
            left_ref = bc.center.x - bc.size.x / 2,
            right_ref = bc.center.x + bc.size.x / 2,
            front_ref = bc.center.z - bc.size.z / 2,
            back_ref = bc.center.z + bc.size.z / 2;
        Vector3 my_point1 = transform.TransformPoint(new Vector3(left_ref, upper_ref, front_ref)),
            my_point2 = transform.TransformPoint(new Vector3(right_ref, upper_ref, front_ref)),
            my_point3 = transform.TransformPoint(new Vector3(left_ref, upper_ref, back_ref)),
            my_point4 = transform.TransformPoint(new Vector3(right_ref, upper_ref, back_ref));

        if (my_point1.x >= my_point2.x)
        {
            float aux = my_point1.x;
            my_point1.x = my_point3.x = my_point2.x;
            my_point2.x = my_point4.x = aux;
        }

        BoxCollider their_bc = myCollider.GetComponent<BoxCollider>();
        upper_ref = their_bc.center.y + their_bc.size.y / 2;
        left_ref = their_bc.center.x - their_bc.size.x / 2;
        right_ref = their_bc.center.x + their_bc.size.x / 2;
        front_ref = their_bc.center.z - their_bc.size.z / 2;
        back_ref = their_bc.center.z + their_bc.size.z / 2;
        Vector3 their_point1 = myCollider.transform.TransformPoint(new Vector3(left_ref, upper_ref, front_ref)),
            their_point2 = myCollider.transform.TransformPoint(new Vector3(right_ref, upper_ref, front_ref)),
            their_point3 = myCollider.transform.TransformPoint(new Vector3(left_ref, upper_ref, back_ref)),
            their_point4 = myCollider.transform.TransformPoint(new Vector3(right_ref, upper_ref, back_ref));

        if (their_point1.x >= their_point2.x)
        {
            float aux = their_point1.x;
            their_point1.x = their_point3.x = their_point2.x;
            their_point2.x = their_point4.x = aux;
        }

        bool case1, case2, case3, case4;
        case1 = case2 = case3 = case4 = false;
        //FINDING CASE OF COLLISION
        //CASE 1 - target upper side collision
        if (my_point1.z <= their_point3.z && my_point3.z >= their_point3.z)
            case1 = true;
        //CASE 2 - target right side collision
        if (my_point1.x <= their_point2.x && my_point2.x >= their_point2.x)
            case2 = true;
        //CASE 3 - target lower side collision
        if (my_point3.z >= their_point1.z && my_point1.z <= their_point1.z)
            case3 = true;
        //CASE 4 - target left side collision
        if (my_point2.x >= their_point1.x && my_point1.x <= their_point1.x)
            case4 = true;

        float dist_x, dist_z;
        //CONSTRAINING MOVEMENT
        if (case1 && case4)
        {
            dist_z = their_point3.z - my_point2.z;
            dist_x = my_point2.x - their_point3.x;
            if (dist_z > dist_x) this_movingright = false;
            else this_movingdown = false;
        }
        else if (case1 && case2)
        {
            dist_z = their_point4.z - my_point1.z;
            dist_x = their_point4.x - my_point1.x;
            if (dist_z > dist_x) this_movingleft = false;
            else this_movingdown = false;
        }
        else if (case2 && case3)
        {
            dist_z = my_point3.z - their_point2.z;
            dist_x = their_point2.x - my_point3.x;
            if (dist_z > dist_x) this_movingleft = false;
            else this_movingup = false;
        }
        else if (case3 && case4)
        {
            dist_z = my_point4.z - their_point1.z;
            dist_x = my_point4.x - their_point1.x;
            if (dist_z > dist_x) this_movingright = false;
            else this_movingup = false;
        }
        else if (case1) this_movingdown = false;
        else if (case2) this_movingleft = false;
        else if (case3) this_movingup = false;
        else if (case4) this_movingright = false;

        //SAVING THE COLLIDER AND FORBIDDEN DIRECTION
        ColliderRestrictions cr = new ColliderRestrictions();
        cr.col = myCollider;
        cr.moving_right = this_movingright;
        cr.moving_left = this_movingleft;
        cr.moving_up = this_movingup;
        cr.moving_down = this_movingdown;
        MovementRestrictionList.Add(cr);

        //APPLYING THE NEW RESTRICTION
        if (!this_movingright) moving_right = false;
        else if (!this_movingleft) moving_left = false;
        else if (!this_movingup) moving_up = false;
        else if (!this_movingdown) moving_down = false;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerStay(Collider myCollider)
    {
        //CHECKING IF CHARACTER IS ATTACKING EFFECTIVELY
        if (onAttack > 0 && myCollider.CompareTag("Player") &&
            ((facing_right && myCollider.transform.position.x > transform.position.x) || (!facing_right && myCollider.transform.position.x < transform.position.x))
            && attack_counter > 0)
        {
            myCollider.SendMessage("ApplyDamage", 1);
            attack_counter = 0;
            if (onAttack == 1) sm.SendMessage("PlayOutLoud", _punch01);
            else if (onAttack == 2) sm.SendMessage("PlayOutLoud", _punch02);
        }
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnTriggerExit(Collider myCollider)
    {
        int i;
        for (i = 0; i < MovementRestrictionList.Count; i++)
        {
            if (MovementRestrictionList[i].col == myCollider) break;
        }
        if (!MovementRestrictionList[i].moving_right) moving_right = true;
        else if (!MovementRestrictionList[i].moving_left) moving_left = true;
        else if (!MovementRestrictionList[i].moving_up) moving_up = true;
        else if (!MovementRestrictionList[i].moving_down) moving_down = true;
        MovementRestrictionList.RemoveAt(i);

        //APPLYING OTHER RESTRICTIONS
        for (i = 0; i < MovementRestrictionList.Count; i++)
        {
            if (!MovementRestrictionList[i].moving_right) moving_right = false;
            else if (!MovementRestrictionList[i].moving_left) moving_left = false;
            else if (!MovementRestrictionList[i].moving_up) moving_up = false;
            else if (!MovementRestrictionList[i].moving_down) moving_down = false;
        }
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public struct ColliderRestrictions
    {
        public Collider col;
        public bool moving_right, moving_left, moving_up, moving_down;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    protected void Flip()
    {
        facing_right = !facing_right;
        // Multiply the player's x local scale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    void OnDestroy()
    {
        enemyCount--;
        if (enemyCount == 0 && gm != null) gm.SendMessage("AllEnemiesKilled");
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void DefineTarget()
    {
        float xtar, ytar;
        float x_aux1, x_aux2, dist1, dist2;
        //Checking shortest distance and saving corresponding position
        x_aux1 = player.transform.TransformPoint(new Vector3(bc_theirs.center.x - bc_theirs.size.x / 2 - 2f, 0, 0)).x;
        x_aux2 = player.transform.TransformPoint(new Vector3(bc_theirs.center.x + bc_theirs.size.x / 2 + 2f, 0, 0)).x;
        dist1 = Mathf.Abs(x_aux1 - transform.position.x);
        dist2 = Mathf.Abs(x_aux2 - transform.position.x);
        if (dist1 <= dist2) xtar = x_aux1;
        else xtar = x_aux2;
        ytar = player.transform.TransformPoint(0, bc_theirs.center.y - bc_theirs.size.y / 2, 0).y;
        target = new Vector3(xtar, ytar, player.transform.position.z);
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public void ReDefineTarget()
    {
        Vector3 frontStart, backStart, frontAimingAt, backAimingAt;
        RaycastHit frontRay01, frontRay02, frontRay03,
            backRay01, backRay02, backRay03;
        bool frontHit, backHit;
        bool target_isright, target_isup;
        float rayRotation = 20f;
        float rayLength = 2f;
        frontHit = backHit = false;
        List<Vector3> NormalList = new List<Vector3>();
        Collider rayCollider = null;
        BoxCollider their_bc;

        //DEFINING STARTING POINTS
        if (target.y >= bc.center.y - bc.size.y / 2) target_isup = true;
        else target_isup = false;
        if (target.x >= bc.center.x - bc.size.x / 2) target_isright = true;
        else target_isright = false;

        frontStart = transform.TransformPoint(new Vector3(bc.center.x + bc.size.x * 0.45f, bc.center.y - bc.size.y * 0.45f, bc.center.z));
        backStart = transform.TransformPoint(new Vector3(bc.center.x - bc.size.x * 0.45f, bc.center.y - bc.size.y * 0.45f, bc.center.z));
        frontAimingAt = (target - frontStart).normalized;
        backAimingAt = (target - backStart).normalized;

        // ---DEBUGGING PURPOSES---
        Debug.DrawRay(frontStart, frontAimingAt * rayLength, Color.red, .25f);
        Debug.DrawRay(frontStart, Quaternion.Euler(0, -rayRotation, 0) * frontAimingAt * rayLength, Color.red, .5f);
        Debug.DrawRay(frontStart, Quaternion.Euler(0, rayRotation, 0) * frontAimingAt * rayLength, Color.red, .5f);

        Debug.DrawRay(backStart, backAimingAt * rayLength, Color.red, .25f);
        Debug.DrawRay(backStart, Quaternion.Euler(0, -rayRotation, 0) * backAimingAt * rayLength, Color.red, .5f);
        Debug.DrawRay(backStart, Quaternion.Euler(0, rayRotation, 0) * backAimingAt * rayLength, Color.red, .5f);
        // ---

        //CHECKING IF THERE IS A FRONT RAY TOUCHING SOMETHING
        if (Physics.Raycast(frontStart, frontAimingAt, out frontRay01, rayLength, obsavoidMask))
        {
            frontHit = true;
            rayCollider = frontRay01.collider;
            if (!NormalList.Contains(frontRay01.normal)) NormalList.Add(frontRay01.normal);
        }
        if (Physics.Raycast(frontStart, Quaternion.Euler(0, -rayRotation, 0) * frontAimingAt, out frontRay02, rayLength, obsavoidMask))
        {
            frontHit = true;
            rayCollider = frontRay02.collider;
            if (!NormalList.Contains(frontRay02.normal)) NormalList.Add(frontRay02.normal);
        }
        if (Physics.Raycast(frontStart, Quaternion.Euler(0, rayRotation, 0) * frontAimingAt, out frontRay03, rayLength, obsavoidMask))
        {
            frontHit = true;
            rayCollider = frontRay03.collider;
            if (!NormalList.Contains(frontRay03.normal)) NormalList.Add(frontRay03.normal);
        }
        //CHECKING IF THERE IS A BACK RAY TOUCHING SOMETHING
        if (Physics.Raycast(backStart, backAimingAt, out backRay01, rayLength, obsavoidMask))
        {
            backHit = true;
            rayCollider = backRay01.collider;
            if (!NormalList.Contains(backRay01.normal)) NormalList.Add(backRay01.normal);
        }
        if (Physics.Raycast(backStart, Quaternion.Euler(0, -rayRotation, 0) * backAimingAt, out backRay02, rayLength, obsavoidMask))
        {
            backHit = true;
            rayCollider = backRay02.collider;
            if (!NormalList.Contains(backRay02.normal)) NormalList.Add(backRay02.normal);
        }
        if (Physics.Raycast(backStart, Quaternion.Euler(0, rayRotation, 0) * backAimingAt, out backRay03, rayLength, obsavoidMask))
        {
            backHit = true;
            rayCollider = backRay03.collider;
            if (!NormalList.Contains(backRay03.normal)) NormalList.Add(backRay03.normal);
        }

        // ---DEBUGGING PURPOSES---
        /*if (frontHit || backHit)
        {
            if (frontHit) print("Algo tapó los rayos de adelante.");
            if (backHit) print("Algo tapó los rayos de atrás.");
            for (int i = 0; i < NormalList.Count; i++)
                print(NormalList[i]);
            Debug.Break();
        }*/
        // ---

        //CASE 1 - Nothing blocks the rays
        if (!backHit && !frontHit)
        {
            Vector3 aux = target - transform.position;
            aux = new Vector3(aux.x, aux.y, 0);
            if (aux.magnitude < 0.25f)
            {
                //target = transform.position + aux;
                //target = transform.position;
                print("No hits. Short.");
                //Debug.Break();
            }
            else
            {
                target = transform.position + aux.normalized * 1 / 4;
                print("No hits. Long.");
                //Debug.Break();
            }
        }
        //CASE 2 - Something blocks the rays from the front or the back
        if (frontHit || backHit)
        {
            their_bc = rayCollider.GetComponent<BoxCollider>();
            Vector3 aux1, aux2;
            aux1 = aux2 = Vector3.zero;
            float dist1, dist2;
            Vector3 nuTarget, nuDirection;
            nuTarget = nuDirection = Vector3.zero;

            for(int i=0; i < NormalList.Count; i++)
            {
                // If ray hit from above...
                if (NormalList[i] == new Vector3(0, 0, 1f))
                {
                    aux1.x = their_bc.center.x - their_bc.size.x / 2 - bc.size.x / 2;
                    aux1.y = their_bc.center.z + their_bc.size.z / 2 + bc.size.z / 2;
                    aux2.x = their_bc.center.x + their_bc.size.x / 2 + bc.size.x / 2;
                    aux2.y = aux1.y;

                    aux1 = rayCollider.transform.TransformPoint(aux1);
                    aux2 = rayCollider.transform.TransformPoint(aux2);

                    dist1 = (target - aux1).magnitude;
                    dist2 = (target - aux2).magnitude;
                    if (dist1 < dist2) nuTarget = aux1;
                    else nuTarget = aux2;

                    nuDirection += (nuTarget - transform.position).normalized;
                }
                // If ray hit from the right
                if (NormalList[i] == new Vector3(1f, 0, 0))
                {
                    aux1.x = their_bc.center.x + their_bc.size.x / 2 + bc.size.x / 2;
                    aux1.y = their_bc.center.z + their_bc.size.z / 2 + bc.size.z / 2;
                    aux2.x = aux1.x;
                    aux2.y = their_bc.center.z - their_bc.size.z / 2 - bc.size.z / 2;

                    aux1 = rayCollider.transform.TransformPoint(aux1);
                    aux2 = rayCollider.transform.TransformPoint(aux2);

                    dist1 = (target - aux1).magnitude;
                    dist2 = (target - aux2).magnitude;
                    if (dist1 < dist2) nuTarget = aux1;
                    else nuTarget = aux2;

                    nuDirection += (nuTarget - transform.position).normalized;
                }
                //If ray hit from below
                if (NormalList[i] == new Vector3(0, 0, -1f))
                {
                    aux1.x = their_bc.center.x - their_bc.size.x / 2 - bc.size.x / 2;
                    aux1.y = their_bc.center.z - their_bc.size.z / 2 - bc.size.z / 2;
                    aux2.x = their_bc.center.x + their_bc.size.x / 2 + bc.size.x / 2;
                    aux2.y = aux1.y;

                    aux1 = rayCollider.transform.TransformPoint(aux1);
                    aux2 = rayCollider.transform.TransformPoint(aux2);

                    dist1 = (target - aux1).magnitude;
                    dist2 = (target - aux2).magnitude;
                    if (dist1 < dist2) nuTarget = aux1;
                    else nuTarget = aux2;

                    nuDirection += (nuTarget - transform.position).normalized;
                }
                // If ray hit from the left
                if (NormalList[i] == new Vector3(-1f, 0, 0))
                {
                    aux1.x = their_bc.center.x - their_bc.size.x / 2 - bc.size.x / 2;
                    aux1.y = their_bc.center.z + their_bc.size.z / 2 + bc.size.z / 2;
                    aux2.x = aux1.x;
                    aux2.y = their_bc.center.z - their_bc.size.z / 2 - bc.size.z / 2;

                    aux1 = rayCollider.transform.TransformPoint(aux1);
                    aux2 = rayCollider.transform.TransformPoint(aux2);

                    dist1 = (target - aux1).magnitude;
                    dist2 = (target - aux2).magnitude;
                    if (dist1 < dist2) nuTarget = aux1;
                    else nuTarget = aux2;

                    nuDirection += (nuTarget - transform.position).normalized;
                }
            }
            nuDirection = nuDirection.normalized;
            //target = nuTarget;
            target = transform.position + nuDirection;
            print("El nuevo goal está en: " + nuTarget);
            print("La nueva dirección de movimiento es: " + nuDirection);
            //Debug.Break();
        }
            
        //CASE 3 - Something blocks the rays from both sides



        /*if (Physics.Raycast(frontStart, frontAimingAt, out frontRay01, 1f, obsavoidMask) ||
            Physics.Raycast(frontStart, Quaternion.Euler(0, -rayRotation, 0) * frontAimingAt, out frontRay02, 1f, obsavoidMask) ||
            Physics.Raycast(frontStart, Quaternion.Euler(0, rayRotation, 0) * frontAimingAt, out frontRay03, 1f, obsavoidMask))
        {
            frontHit = true;
            print("Impactó rayo delantero.");
            Debug.Break();
        }
        if (Physics.Raycast(backStart, backAimingAt, out backRay01, 1f, obsavoidMask) ||
            Physics.Raycast(backStart, Quaternion.Euler(0, -rayRotation, 0) * backAimingAt, out backRay02, 1f, obsavoidMask) ||
            Physics.Raycast(backStart, Quaternion.Euler(0, rayRotation, 0) * backAimingAt, out backRay03, 1f, obsavoidMask))
        {
            backHit = true;
            print("Impactó rayo trasero.");
            Debug.Break();
        }*/

        /*
        //CASE 1 - Nothing blocks the rays
        if (!backHit && !frontHit)
        {
            //target = transform.position + frontAimingAt;
            Vector3 aux = target - frontStart;
            aux = new Vector3(aux.x, aux.y, 0);
            if (aux.magnitude < 1.5f)
            {
                //target = transform.position + aux;
                //target = transform.position;
                print("No hits. Short. aux is: " + aux);
                //Debug.Break();
            }
            else
            {
                target = transform.position + aux.normalized * 1 / 5;
                print("No hits. Long.");
                //Debug.Break();
            }
        }
        */

        /*
        Vector3 frontStart, backStart, frontAimingAt, backAimingAt;
        //Vector3 target_aux;
        bool frontHit, backHit;
        bool target_isright, target_isup;
        float rotation = 20f;
        frontHit = backHit = false;

        //DEFINING STARTING POINTS
        if (target.y >= bc.center.y - bc.size.y / 2) target_isup = true;
        else target_isup = false;
        if (target.x >= bc.center.x - bc.size.x / 2) target_isright = true;
        else target_isright = false;

        frontStart = transform.TransformPoint(new Vector3(bc.center.x + bc.size.x / 2, bc.center.y - bc.size.y * 0.45f, bc.center.z));
        backStart = transform.TransformPoint(new Vector3(bc.center.x - bc.size.x / 2, bc.center.y - bc.size.y * 0.45f, bc.center.z));
        frontAimingAt = (target - frontStart).normalized;
        backAimingAt = (target - backStart).normalized;

        // ---DEBUGGING PURPOSES---
        Debug.DrawRay(frontStart, frontAimingAt, Color.red, .25f);
        //Debug.DrawRay(frontStart, Quaternion.Euler(0, -rotation, 0) * frontAimingAt, Color.red, .5f);
        //Debug.DrawRay(frontStart, Quaternion.Euler(0, rotation, 0) * frontAimingAt, Color.red, .5f);

        Debug.DrawRay(backStart, backAimingAt, Color.red, .25f);
        //Debug.DrawRay(backStart, Quaternion.Euler(0, -rotation, 0) * backAimingAt, Color.red, .5f);
        //Debug.DrawRay(backStart, Quaternion.Euler(0, rotation, 0) * backAimingAt, Color.red, .5f);
        // ---

        //CHECKING IF THERE IS A FRONT RAY TOUCHING SOMETHING
        frontHit = CheckAvoidanceCone(frontStart, frontAimingAt, rotation);
        if (frontHit) print("There is a front ray touching something");
        //CHECKING IF THERE IS A BACK RAY TOUCHING SOMETHING
        backHit = CheckAvoidanceCone(backStart, backAimingAt, rotation);
        if (backHit) print("There is a back ray touching something");

        //RaycastHit rch;
        //Physics.Raycast(frontStart, direction, out rch, 1f, obsavoidMask);
        //rch.normal;

        print("Previous target at: " + target);
        print("frontStart is: " + frontStart + " and frontAimingAt is: " + frontAimingAt);

        //CASE 1 - Nothing blocks the rays
        if (!backHit && !frontHit)
        {
            //target = transform.position + frontAimingAt;
            Vector3 aux = target - frontStart;
            aux = new Vector3(aux.x, aux.y, 0);
            if (aux.magnitude < 1.5f)
            {
                //target = transform.position + aux;
                //target = transform.position;
                print("No hits. Short. aux is: " + aux);
                //Debug.Break();
            }
            else
            {
                target = transform.position + aux.normalized*1/5;
                print("No hits. Long.");
                //Debug.Break();
            }
        }

        //CASE 2 - Something blocks the rays from the front
        else if (!backHit && frontHit)
        {
            // If target is to my left
            if (!target_isright)
            {
                // ...then I move it to my right
                if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0.5f, 0, 0)).normalized, rotation))
                {
                    target = transform.position + (frontAimingAt + new Vector3(0.5f, 0, 0)).normalized;
                    print("Here! 01");
                    Debug.Break();
                }
                // If I hit something, I try again moving it up or down depending on whether the target is down or up, respectively
                // Now I move it down, because the target is up
                else if (target_isup)
                {
                    if (!CheckAvoidanceCone(frontStart, frontAimingAt + new Vector3(0, -1f, 0), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, -1f, 0));
                        print("Here! 02");
                        Debug.Break();
                    }
                    // If I hit something again, I try combining the two directions
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0.5f, -1f, 0)).normalized, rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0.5f, -1f, 0));
                        print("Here! 03");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my left
                    else
                    {
                        target = transform.position + Vector3.left/2;
                        print("Here! 04");
                        Debug.Break();
                    } 
                }
                // target is down, so I move it up
                else
                {
                    if (!CheckAvoidanceCone(frontStart, frontAimingAt + new Vector3(0, 1f, 0), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, 1f, 0));
                        print("Here! 05");
                        Debug.Break();
                    }
                    // If I hit something again, I try combining the two directions
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0.5f, 1f, 0)).normalized, rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0.5f, 1f, 0));
                        print("Here! 06");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my left
                    else
                    {
                        target = transform.position + Vector3.left/2;
                        print("Here! 07");
                        Debug.Break();
                    }
                }
            }
            // If target is to my right
            else
            {
                // ...then I move it to my left
                if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, 0, 0)).normalized, rotation))
                {
                    target = transform.position + (frontAimingAt + new Vector3(-0.5f, 0, 0)).normalized;
                    print("Here! 08");
                    Debug.Break();
                }
                // If I hit something, I try again moving it up or down depending on whether the target is down or up, respectively
                // Now I move it down, because the target is up
                else if (target_isup)
                {
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, -1f, 0));
                        print("Here! 09");
                        Debug.Break();
                    }
                    // If I hit something again, I try combining the two directions
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-0.5f, -1f, 0));
                        print("Here! 10");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my right
                    else
                    {
                        target = transform.position + Vector3.right/2;
                        print("Here! 11");
                        Debug.Break();
                    }
                }
                // target is down, so I move it up
                else
                {
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, 1f, 0));
                        print("Here! 12");
                        Debug.Break();
                    }
                    // If I hit something again, I try combining the two directions
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-0.5f, 1f, 0));
                        print("Here! 13");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my left
                    else
                    {
                        target = transform.position + Vector3.left/2;
                        print("Here! 14");
                        Debug.Break();
                    }
                }
            }
        }

        //CASE 3 - Something blocks the rays from the back
        else if (backHit && !frontHit)
        {
            // If target is to my left
            if (!target_isright)
            {
                // ...then I move it to my left
                if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, 0, 0)).normalized, rotation))
                {
                    target = transform.position + (frontAimingAt + new Vector3(-0.5f, 0, 0)).normalized;
                    print("Here! 15");
                    Debug.Break();
                }
                // If I hit something, I try again moving it up or down depending on whether the target is down or up, respectively
                // Now I move it down, because the target is up
                else if (target_isup)
                {
                    // If I hit something again, I try combining the two directions
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-0.5f, -1f, 0));
                        print("Here! 17");
                        Debug.Break();
                    }
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, -1f, 0));
                        print("Here! 16");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my left
                    else
                    {
                        target = transform.position + Vector3.left/4;
                        print("Here! 18");
                        Debug.Break();
                    }
                }
                // target is down, so I move it up
                else
                {
                    // If I hit something again, I try combining the two directions
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-0.5f, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-0.5f, 1f, 0));
                        print("Here! 20");
                        Debug.Break();
                    }
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, 1f, 0));
                        print("Here! 19");
                        Debug.Break();
                    }
                    // If I hit something again, then I simply try to move to my right
                    else
                    {
                        target = transform.position + Vector3.left/2;
                        print("Here! 21");
                        Debug.Break();
                    }
                }
            }
            // If target is to my right
            else
            {
                // ...then I move it to my right
                if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0.5f, 0, 0)).normalized, rotation))
                {
                    target = transform.position + (frontAimingAt + new Vector3(0.5f, 0, 0)).normalized;
                    print("Here! 22");
                    Debug.Break();
                }
                // If I hit something, I try again moving it up or down depending on whether the target is down or up, respectively
                // Now I move it down, because the target is up
                else if (target_isup)
                {
                    //If I hit something again, I try combining the two directions
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-2f, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-2f, -1f, 0));
                        print("Here! 24");
                        Debug.Break();
                    }
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, -1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, -1f, 0));
                        print("Here! 23");
                        Debug.Break();
                    }
                    //If I hit something again, then I simply try to move to my left
                    else 
                    {
                        target = transform.position + Vector3.left;
                        print("Here! 25");
                        Debug.Break();
                    }
                }
                // target is down, so I move it up
                else
                {
                    //If I hit something again, I try combining the two directions
                    if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(-2f, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(-2f, 1f, 0));
                        print("Here! 27");
                        Debug.Break();
                    }
                    else if (!CheckAvoidanceCone(frontStart, (frontAimingAt + new Vector3(0, 1f, 0)), rotation))
                    {
                        target = transform.position + (frontAimingAt + new Vector3(0, 1f, 0));
                        print("Here! 26");
                        Debug.Break();
                    }
                    //If I hit something again, then I simply try to move to my left
                    else
                    {
                        target = transform.position + Vector3.left;
                        print("Here! 28");
                        Debug.Break();
                    }
                }
            }
        }

        //CASE 4 - Something blocks the rays from both sides
        else if (backHit && frontHit)
        {
            // If target is to my right...
            if (target_isright)
            {
                // ...then I try to move to the left
                if (!CheckAvoidanceCone(frontStart, Vector3.left, rotation))
                {
                    target = transform.position + Vector3.left * 2.5f;
                    print("Here! 29");
                    Debug.Break();
                }
                // if I can't, then I move to the right
                else
                {
                    target = transform.position + Vector3.right * 2.5f;
                    print("Here! 30");
                    Debug.Break();
                }
            }
            // If the target is to my left...
            else
            {
                // ...then I try to move to the right
                if (!CheckAvoidanceCone(frontStart, Vector3.right, rotation))
                {
                    target = transform.position + Vector3.right * 2.5f;
                    print("Here! 31");
                    Debug.Break();
                }
                // if I can't, then I move to the left
                else
                {
                    target = transform.position + Vector3.left * 2.5f;
                    print("Here! 32");
                    Debug.Break();
                }
            }
        }    
        print("Next target is: " + target);
        */
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    public bool CheckAvoidanceCone(Vector3 startPoint, Vector3 direction, float rot)
    {
        return Physics.Raycast(startPoint, direction, 1f, obsavoidMask) ||
            Physics.Raycast(startPoint, Quaternion.Euler(0, -rot, 0) * direction, 1f, obsavoidMask) ||
            Physics.Raycast(startPoint, Quaternion.Euler(0, rot, 0) * direction, 1f, obsavoidMask);
    }
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    /* -------------------------------------------------------------------------------------------------------------------------------- */
    /* -------------------------------------------------------------------------------------------------------------------------------- */
}
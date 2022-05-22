public static TransporterManager instance;
private Queue<ResourceObject> orders = new Queue<ResourceObject>();

private void Awake()
{
    //If the static instance variable is set
    if (instance != null && instance != this)
    {
        //Then destroy the instance
        Destroy(instance.gameObject);
    }
    
    //Set the instance
    instance = this;
}

private void Update()
{
    //return if no orders are found
    if (orders.Count <= 0) { return; }

    //Get the first order
    ResourceObject order = orders.Dequeue();

    //Find a transporter
    SettlerBaseBehaviour transporter = FindTransporter(order);
    if (transporter != null)
    {
        //get carry amount from the transporter
        int carryAmount = transporter.GetCarryAmount();

        //If the Transporter cannot carry everything from the order
        if (carryAmount < order.GetAmount())
        {
            //split the order
            SplitOrder(order, order.GetAmount() - carryAmount);
            order.ChangeAmount(-(order.GetAmount() - carryAmount));
        }

        List<AnimationStep> animation = new List<AnimationStep>();
        //... (generates the animation)
        //Set the order for the transporter
        transporter.transporterState.SetOrder(transporter, new ResourceObject(order), animation);
    }
    else
    {
        //No transporter found. Re-adding the order at the back of the queue
        AddOrder(new ResourceObject(order));
    }
}

//find that contains resource type
private SettlerBaseBehaviour FindTransporter(ResourceObject order)
{
    SettlerBaseBehaviour closestTransporter = null;
    float minDistance = float.MaxValue;

    //Get all settlers
    foreach (GameObject settler in DataManager.settlers)
    {
        SettlerBaseBehaviour settlerBase = settler.GetComponent<SettlerBaseBehaviour>();

        //Only continue if the settler is a transporter and if the transporter is available
        if (settlerBase != null && settlerBase.settlerType == SettlerType.Transporter && settlerBase.IsIdle())
        {
            //Get the distance between the order source and the transporter (to find the closest transporter)
            float distance = Vector3.Distance(order.GetSource().transform.position, settler.transform.position);

            //If the distance is less then store this one
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTransporter = settlerBase;
            }
        }
    }

    //return the found transporter (if none found then it returns null)
    return closestTransporter;
}

//Creates a new order from the old one and reduces the amount
private void SplitOrder(ResourceObject order, int newAmount)
{
    AddOrder(new ResourceObject(order.GetID(), order.GetResourceType(), newAmount, order.GetDestination(), order.GetSource(), order.GetTransporter()));
}

//Adds the order to the queue
public void AddOrder(ResourceObject order)
{
    orders.Enqueue(order);
}

//Removes the order from the queue
public void CancelOrder(BuildingBaseBehaviour target)
{
    orders = new Queue<ResourceObject>(orders.Where(x => x.GetSource() != target.gameObject && x.GetDestination() != target.gameObject));
}

//Overrides the ToString method with a custom one
public new string ToString()
{
    string result = "";
    foreach (ResourceObject item in orders)
    {
        result += "Order: " + item.ToString() + "\n";
    }
    return result;
}
local gameObject;
local userName;
local password;


function GetUI(ui)
	gameObject=ui;
	userName=gameObject.Find("InputUserName"):GetComponent("InputField");
	password=gameObject.Find("InputPassword"):GetComponent("InputField");
	gameObject.Find("BtnLogin"):GetComponent("Button").onClick:AddListener(OnLoginClick);
	--UpdateBeat:Add(UpdateData, self)
end

function UpdateData()			
	print("---------每帧执行一次");	
end

function OnLoginClick()
	if password.text~="1123" then
		print("please input the correct password!");
		return;
	end
	
	if	userName.text=="sea" and password.text=="1123" then
	print("Input the correct!");
	end	
	
	--local jax=Resource.Load("Cube");
end
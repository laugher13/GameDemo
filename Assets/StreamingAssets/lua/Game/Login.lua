require "Game/SetName"

local gameObject;
local transform;
local account;
local password;


function GetUI(ui)	
	gameObject=ui;	
	transform=gameObject.transform;
	SetName.GetUI(transform);
	account=gameObject.Find("Login/InputAccount"):GetComponent("InputField");
	password=gameObject.Find("Login/InputPassword"):GetComponent("InputField");
	gameObject.Find("Login/BtnLogin"):GetComponent("Button").onClick:AddListener(OnLoginClick);
	--UpdateBeat:Add(UpdateData, self)
end

function UpdateData()			
	print("---------每帧执行一次");	
end

function OnLoginClick()
	account.transform.parent.gameObject:SetActive(false);
	transform:FindChild("SelectName").gameObject:SetActive(true);
	if password.text~="1123" then
		print("please input the correct password!");
		return;
	end
	
	if	account.text=="sea" and password.text=="1123" then
	print("Input the correct!");
	end	
end
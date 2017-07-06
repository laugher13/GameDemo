local gameObject;
local transform;
local userName;
SetName={};

local name={"博枫影月","末世岛屿","一丝苦笑","淡墨文竹","墨羽尘曦","雨博韵潇","泪染倾城","羽之蝶幻","半夏如烟","绯色丶浮華"};

function SetName.GetUI(ui)
	transform=ui;
	userName=transform:FindChild("SelectName/InputUserName"):GetComponent("InputField");
	transform:FindChild("SelectName/BtnGenerate"):GetComponent("Button").onClick:AddListener(SetName.OnGenerateClick);
	transform:FindChild("SelectName/BtnConfirm"):GetComponent("Button").onClick:AddListener(SetName.OnConfirmClick);
end

function SetName.OnGenerateClick()
	local num= math.random(1,10);
	userName.text=name[num];
	print(name[num]);
end

function SetName.OnConfirmClick()
	if userName.text==nil or userName.text=="" then
		print("username cannot be nil");
		return;
	end
	SetName.HideUI();
end

function SetName.ShowUI()
	userName.transform.parent.gameObject.SetActive(true);
end

function SetName.HideUI()
	userName.transform.parent.gameObject:SetActive(false);
end

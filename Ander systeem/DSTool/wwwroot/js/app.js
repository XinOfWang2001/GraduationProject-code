window.HelloWorld = () => {
    console.log("Hello world")
}

window.HelloName= (input)=>{
    console.log("Hello world" + " " + input)
};

var dotnet;

window.InitDotnet = (dotnetRef) => {
    dotnet = dotnetRef;

}

window.CallDotnet = () => {
    dotnet.invokeMethodAsync('DotnetFromJs')
}
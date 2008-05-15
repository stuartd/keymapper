// On page load, hide the page elements that should be hidden.
// If scripts are disabled, the page elements will all be shown and the text to show them is itself hidden.

function toggleAnswers()
{
  
   var list=document.getElementById('ScenarioList').getElementsByTagName('span');
    
     for(var i=0;i<list.length;i++)
    {
     list[i].className = (list[i].className=='hideanswer')?'showanswer':'hideanswer' ;
     }
     
     return false ;
    
}


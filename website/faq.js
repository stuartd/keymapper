function toggleAnswers()
{
  
   var list=document.getElementById('ScenarioList').getElementsByTagName('span');
    
     for(var i=0;i<list.length;i++)
    {
     list[i].className = (list[i].className=='hideanswer')?'showanswer':'hideanswer' ;
     }
     
     return false ;
    
}

function load()
{
    // By default - ie without scripting - all elements are shown and the text to show them is itself hidden.
    // In this function, then, call the function to toggle the values ..
    toggleAnswers() ;
    // .. and switch the class of the text.
    var txt = document.getElementById('hideanswerstext') ;
    txt.id = 'showanswerstext' ;

}







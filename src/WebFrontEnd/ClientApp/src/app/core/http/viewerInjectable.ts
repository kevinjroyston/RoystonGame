
import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { OnInit } from '@angular/core';

//Used to create a single instance of the unity viewer

@Injectable({
    providedIn: 'root'
})
export class UnityViewer{

    gameInstance: any;
    containerId: string="";
    viewerDiv: Element = null;
    private lobbyId = null;
    private currentLobbyId = null;
    public progress = 0;
    public isReady = false;
  
    private renderer: Renderer2;
    constructor (rendererFactory: RendererFactory2) {
        // Get an instance of Angular's Renderer2
        this.renderer = rendererFactory.createRenderer(null, null);
    }

    FixBlurriness(){
        var canvas = this.gameInstance.Module.canvas;
        var container = this.gameInstance.container;

//        container.style.width = '960px';//canvas.style.width;
//        container.style.height = '540px'; //canvas.style.height;            
//        container.style.width = canvas.style.width;
        var el = document.getElementById(this.containerId);
        if (Math.round((el.clientHeight-1)*96/54) <= el.clientWidth) {
          container.style.width = Math.round((el.clientHeight-1)*96/54)+'px';
          container.style.height = el.clientHeight-1+'px';
        }
        else
        {
          container.style.width = el.clientWidth+'px';
          container.style.height = Math.round((el.clientWidth)*54/96)+'px';
        }
    }

    showFullScreen(){
        if (this.gameInstance) {
            this.gameInstance.SetFullscreen(1);
        }
    }
    UpdateLobbyId(lobbyId) {
        this.lobbyId = lobbyId;
        if (lobbyId == this.currentLobbyId) return;

        if (this.gameInstance) {
            this.currentLobbyId = this.lobbyId;
            console.log('Sending lobby id to viewer');
            this.gameInstance.SendMessage("JavascriptConnector","ConnectToLobby",this.lobbyId);
            setTimeout(() => {
               this.FixBlurriness(); 
            }, 6000);
/**/

/*            
            var w = window.innerWidth - 20;
            var h = window.innerHeight - 40;
            var r = 1080 / 1920;
            if (w * r > h) {
                w = Math.min(w,
                    Math.ceil(h / r));
            }
            h = Math.floor(w * r);

            container.style.width = w + "px";
            container.style.height = h + "px";            
*/            
        }
    }

    createDIV() {
        // Use Renderer2 to create the div element
        this.viewerDiv = this.renderer.createElement('div');
        // Set the id of the div
        this.renderer.setProperty(this.viewerDiv, 'id', 'viewerContainer');

        this.renderer.appendChild(document.body, this.viewerDiv);

    }

    InitializeViewer(containerId): void {
        this.containerId = containerId;
        if (!this.viewerDiv) {  // first call create a div and initialize the viewer
            this.createDIV();  
            const loader = (window as any).UnityLoader;
  
            this.gameInstance = loader.instantiate(
                'viewerContainer', 
                '/viewer/Build/ScrawlBrawlWebViewer.json', {
            onProgress: (gameInstance: any, progress: number) => {
                  this.progress = progress;
                    if (progress === 1) {
                        this.isReady = true;
                        if (this.lobbyId) {
                            this.UpdateLobbyId(this.lobbyId);
                        }
                    }
                }
            });
        }   
        // Append the div to the body element
        this.renderer.appendChild(document.getElementById(containerId), this.viewerDiv);
    }
     
}
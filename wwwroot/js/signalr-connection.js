// SignalR Real-time Connection for ScrapeWise
// Manages real-time updates for scraping jobs

class ScrapeWiseSignalR {
    constructor() {
        this.connection = null;
        this.isConnected = false;
        this.init();
    }

    // Initialize SignalR connection
    async init() {
        try {
            // Create SignalR connection
            this.connection = new signalR.HubConnectionBuilder()
                .withUrl("/scrapingHub")
                .withAutomaticReconnect()
                .build();

            // Set up event handlers
            this.setupEventHandlers();

            // Start connection
            await this.connection.start();
            this.isConnected = true;
            console.log("✅ SignalR Connected Successfully");
            this.showToast("Connected to real-time updates", "success");

        } catch (error) {
            console.error("❌ SignalR Connection Error:", error);
            this.showToast("Real-time updates unavailable", "warning");
        }
    }

    // Set up all SignalR event handlers
    setupEventHandlers() {
        // Job Started Event
        this.connection.on("JobStarted", (jobId, url, userName) => {
            console.log(`🚀 Job ${jobId} started by ${userName}: ${url}`);
            this.handleJobStarted(jobId, url, userName);
        });

        // New Result Event
        this.connection.on("NewResult", (jobId, extractedText, resultCount) => {
            console.log(`📄 New result for Job ${jobId} (${resultCount}): ${extractedText.substring(0, 50)}...`);
            this.handleNewResult(jobId, extractedText, resultCount);
        });

        // Job Completed Event
        this.connection.on("JobCompleted", (jobId, totalResults) => {
            console.log(`✅ Job ${jobId} completed with ${totalResults} results`);
            this.handleJobCompleted(jobId, totalResults);
        });

        // Job Failed Event
        this.connection.on("JobFailed", (jobId, error) => {
            console.log(`❌ Job ${jobId} failed: ${error}`);
            this.handleJobFailed(jobId, error);
        });

        // Connection events
        this.connection.onreconnecting(() => {
            console.log("🔄 SignalR Reconnecting...");
            this.showToast("Reconnecting to real-time updates...", "info");
        });

        this.connection.onreconnected(() => {
            console.log("✅ SignalR Reconnected");
            this.showToast("Reconnected to real-time updates", "success");
        });

        this.connection.onclose(() => {
            console.log("❌ SignalR Connection Closed");
            this.isConnected = false;
            this.showToast("Real-time updates disconnected", "danger");
        });
    }

    // Handle job started event
    handleJobStarted(jobId, url, userName) {
        // Create or update job progress card
        this.createJobProgressCard(jobId, url, userName);
        
        // Show toast notification
        this.showToast(`🚀 Scraping started: ${url}`, "info");
        
        // If on dashboard, refresh job list after a short delay
        if (window.location.pathname.includes('/Jobs') || window.location.pathname.includes('/Scraper')) {
            // Add visual feedback that a new job is running
            this.addRunningJobIndicator(jobId);
        }
    }

    // Handle new result event
    handleNewResult(jobId, extractedText, resultCount) {
        // Update progress counter
        this.updateJobProgress(jobId, resultCount);
        
        // Add result to live results area if exists
        this.addLiveResult(jobId, extractedText, resultCount);
        
        // If we're on the create page, show the progress
        if (window.location.pathname.includes('/Create')) {
            this.updateCreatePageProgress(resultCount, extractedText);
        }
    }

    // Handle job completed event
    handleJobCompleted(jobId, totalResults) {
        // Update job status to completed
        this.updateJobStatus(jobId, 'completed', totalResults);
        
        // Show success notification
        this.showToast(`✅ Scraping completed! Found ${totalResults} results`, "success");
        
        // If on dashboard, refresh the page after a delay to show new job
        if (window.location.pathname.includes('/Jobs') || window.location.pathname.includes('/Scraper')) {
            setTimeout(() => {
                window.location.reload();
            }, 2000);
        }
        
        // If on create page, redirect to dashboard
        if (window.location.pathname.includes('/Create')) {
            setTimeout(() => {
                window.location.href = '/Jobs';
            }, 2000);
        }
    }

    // Handle job failed event
    handleJobFailed(jobId, error) {
        // Update job status to failed
        this.updateJobStatus(jobId, 'failed', 0);
        
        // Show error notification
        this.showToast(`❌ Scraping failed: ${error}`, "danger");
    }

    // Create job progress card for real-time monitoring
    createJobProgressCard(jobId, url, userName) {
        const progressArea = document.getElementById('live-progress-area');
        if (!progressArea) return;

        const card = document.createElement('div');
        card.id = `job-progress-${jobId}`;
        card.className = 'card mb-3';
        card.style.background = 'rgba(138, 43, 226, 0.1)';
        card.style.borderLeft = '4px solid #8a2be2';
        
        card.innerHTML = `
            <div class="card-body">
                <h6 class="card-title">
                    <i class="fas fa-spinner fa-spin text-primary"></i>
                    Scraping Job #${jobId}
                    <span class="badge bg-primary ms-2">Running</span>
                </h6>
                <p class="card-text">
                    <strong>URL:</strong> ${url}<br>
                    <strong>Started by:</strong> ${userName}
                </p>
                <div class="progress mb-2">
                    <div class="progress-bar progress-bar-striped progress-bar-animated" 
                         role="progressbar" style="width: 0%">
                        <span id="progress-text-${jobId}">Starting...</span>
                    </div>
                </div>
                <div id="live-results-${jobId}" class="mt-2" style="max-height: 200px; overflow-y: auto; background: rgba(0,0,0,0.2); padding: 8px; border-radius: 4px;">
                    <small class="text-muted">Results will appear here as they're found...</small>
                </div>
            </div>
        `;
        
        progressArea.appendChild(card);
    }

    // Update job progress
    updateJobProgress(jobId, resultCount) {
        const progressBar = document.querySelector(`#job-progress-${jobId} .progress-bar`);
        const progressText = document.getElementById(`progress-text-${jobId}`);
        
        if (progressBar && progressText) {
            // Animate progress bar (we don't know total, so we'll use a cycling animation)
            progressBar.style.width = Math.min(resultCount * 2, 95) + '%';
            progressText.textContent = `Found ${resultCount} results...`;
        }
    }

    // Add live result to display
    addLiveResult(jobId, extractedText, resultCount) {
        const resultsContainer = document.getElementById(`live-results-${jobId}`);
        if (!resultsContainer) return;

        // Clear initial message
        if (resultCount === 1) {
            resultsContainer.innerHTML = '';
        }

        const resultElement = document.createElement('div');
        resultElement.className = 'border-bottom pb-1 mb-1';
        resultElement.innerHTML = `
            <small class="text-muted">#${resultCount}</small>
            <div style="font-size: 0.85em;">${extractedText.length > 100 ? extractedText.substring(0, 100) + '...' : extractedText}</div>
        `;
        
        resultsContainer.appendChild(resultElement);
        resultsContainer.scrollTop = resultsContainer.scrollHeight;
    }

    // Update job status
    updateJobStatus(jobId, status, totalResults) {
        const jobCard = document.getElementById(`job-progress-${jobId}`);
        if (!jobCard) return;

        const statusBadge = jobCard.querySelector('.badge');
        const icon = jobCard.querySelector('.fas');
        const progressBar = jobCard.querySelector('.progress-bar');
        const progressText = document.getElementById(`progress-text-${jobId}`);

        if (status === 'completed') {
            statusBadge.className = 'badge bg-success ms-2';
            statusBadge.textContent = 'Completed';
            icon.className = 'fas fa-check-circle text-success';
            progressBar.style.width = '100%';
            progressBar.className = 'progress-bar bg-success';
            if (progressText) progressText.textContent = `Completed - ${totalResults} results found`;
        } else if (status === 'failed') {
            statusBadge.className = 'badge bg-danger ms-2';
            statusBadge.textContent = 'Failed';
            icon.className = 'fas fa-exclamation-circle text-danger';
            progressBar.className = 'progress-bar bg-danger';
        }
    }

    // Update create page progress
    updateCreatePageProgress(resultCount, extractedText) {
        const progressArea = document.getElementById('create-progress');
        if (!progressArea) return;

        progressArea.innerHTML = `
            <div class="alert alert-info">
                <i class="fas fa-spinner fa-spin"></i>
                <strong>Scraping in progress...</strong>
                <br>Found ${resultCount} results so far.
                <br><small>Latest: ${extractedText.substring(0, 80)}...</small>
            </div>
        `;
    }

    // Add running job indicator
    addRunningJobIndicator(jobId) {
        // Add a small indicator that shows jobs are running
        const indicator = document.getElementById('running-jobs-indicator');
        if (indicator) {
            indicator.style.display = 'block';
            indicator.innerHTML = `<i class="fas fa-spinner fa-spin text-primary"></i> Scraping in progress...`;
        }
    }

    // Show toast notification
    showToast(message, type = 'info') {
        // Create toast container if it doesn't exist
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.style.cssText = `
                position: fixed;
                top: 20px;
                right: 20px;
                z-index: 9999;
                min-width: 300px;
            `;
            document.body.appendChild(toastContainer);
        }

        // Create toast element
        const toast = document.createElement('div');
        toast.className = `alert alert-${type} alert-dismissible fade show`;
        toast.style.cssText = `
            margin-bottom: 10px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            border: none;
            background: rgba(255,255,255,0.1);
            backdrop-filter: blur(10px);
            color: white;
        `;
        
        toast.innerHTML = `
            ${message}
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="alert"></button>
        `;

        toastContainer.appendChild(toast);

        // Auto-remove after 5 seconds
        setTimeout(() => {
            if (toast.parentNode) {
                toast.remove();
            }
        }, 5000);
    }
}

// Initialize SignalR when page loads
document.addEventListener('DOMContentLoaded', () => {
    // Only initialize if we're on a relevant page
    const currentPath = window.location.pathname.toLowerCase();
    if (currentPath.includes('/jobs') || currentPath.includes('/scraper') || currentPath.includes('/home')) {
        window.scrapeWiseSignalR = new ScrapeWiseSignalR();
    }
});

// Global function to check connection status
window.checkSignalRStatus = function() {
    if (window.scrapeWiseSignalR) {
        return {
            connected: window.scrapeWiseSignalR.isConnected,
            connectionState: window.scrapeWiseSignalR.connection?.connectionState || 'Unknown'
        };
    }
    return { connected: false, connectionState: 'Not initialized' };
};

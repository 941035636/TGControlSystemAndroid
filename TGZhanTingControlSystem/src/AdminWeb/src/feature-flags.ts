export type AdminWorkspaceView = 'content' | 'routes' | 'versions' | 'operations'

// These switches hide only the user interface. Route APIs, data and source code remain intact.
export const adminUiFeatures = Object.freeze({
  enableCustomRouteUi: false,
  enableThemeCombinationUi: false,
})

export function isAdminWorkspaceViewEnabled(view: AdminWorkspaceView): boolean {
  return view !== 'routes' || adminUiFeatures.enableCustomRouteUi
}

export function normalizeAdminWorkspaceView(view: AdminWorkspaceView): AdminWorkspaceView {
  return isAdminWorkspaceViewEnabled(view) ? view : 'content'
}

export function showRouteBasedTouchTemplates(): boolean {
  return adminUiFeatures.enableCustomRouteUi && adminUiFeatures.enableThemeCombinationUi
}

import test from 'node:test'
import assert from 'node:assert/strict'
import { adminUiFeatures, isAdminWorkspaceViewEnabled, normalizeAdminWorkspaceView, showRouteBasedTouchTemplates } from '../src/feature-flags.ts'

test('route and theme-combination user interfaces are hidden by default', () => {
  assert.equal(adminUiFeatures.enableCustomRouteUi, false)
  assert.equal(adminUiFeatures.enableThemeCombinationUi, false)
  assert.equal(showRouteBasedTouchTemplates(), false)
})

test('an old route-management view safely returns to content management', () => {
  assert.equal(isAdminWorkspaceViewEnabled('routes'), false)
  assert.equal(normalizeAdminWorkspaceView('routes'), 'content')
  assert.equal(normalizeAdminWorkspaceView('versions'), 'versions')
})

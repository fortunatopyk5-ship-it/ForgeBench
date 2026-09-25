# Balaban-native matching state space audit — Iteration 4

Scope: mathematical validity of the matching state space for YM-MATCH-EXTRACT-1 only. STEP_0 is intentionally out of scope.

## 1. Coefficient product is not yet a space of effective densities

Let Adm_k denote the exact set of Balaban-admissible domain histories at RG scale k. Its primary-source definition has NOT yet been directly verified in this run; therefore admissibility/nesting conditions are not invented here.

For each admissible history Sigma and local label i=(sector,j,X,z,...) let H_i be the Banach space of bounded holomorphic local functions on the source analytic domain U_i, with the H-infinity norm. Let d_i be the source localization metric. Define the pointwise coefficient space

B_pt(kappa_*) =
 {F=(F_i): sup_i exp(kappa_* d_i) ||F_i||_{H_i} < infinity}.

This is a Banach space, but an arbitrary F in B_pt need not reconstruct a legitimate Balaban density.

The source density has the schematic nonlinear form

rho_k = Rec_k(c,F)
      = sum_{Sigma in Adm_k}
          chi_Sigma T_Sigma [ exp A_Sigma(c,F) ].

Therefore the true state must include reconstruction, not only coefficients.

## 2. Linear compatibility layer

The following constraints are linear whenever they can be formulated with bounded linear maps on the local H_i spaces:

1. index only source-admissible histories;
2. support/locality: every local activity is intrinsically a function only of the source-restricted field variables;
3. gauge invariance/covariance;
4. Euclidean covariance when required;
5. normalization conditions such as F_i(1)=0;
6. any genuine source restriction/overlap consistency equation of the form
   R_ij F_i = R_ji F_j.

Admissibility and nesting are best encoded in the index set Adm_k rather than as equations on F.

Define C_lin as the intersection of all verified bounded linear compatibility kernels. If the family of compatibility operators C_a is bounded, then

C_lin = intersection_a ker C_a

is a closed linear subspace of B_pt and hence Banach.

This is YM-COMPAT-CLOSED-1 below.

Important: the exact cross-history restriction/overlap equalities required by Balaban have NOT been extracted from primary text. They must not be guessed. For now C_lin contains only those compatibility conditions whose source meaning has been verified.

## 3. Actual-density state is a nonlinear graph

Let D_k be a Banach space in which the reconstructed density is to live (for example an appropriate bounded local-density space once history summability is supplied).

Given a continuous reconstruction map

Rec_k : I_c x C_lin -> D_k,

define

M_adm,k =
 {(c,F,rho):
    c in I_c,
    F in C_lin,
    rho = Rec_k(c,F),
    rho >= 0,
    N(rho)=1}.

Because of exponentiation, sector integration T_Sigma, positivity and normalization, M_adm,k is generally NOT a linear subspace. It is a constrained nonlinear graph (or chart/manifold-like subset).

If Rec_k is continuous, the graph condition is closed. Positivity and a continuous normalization N(rho)=1 are closed constraints. Hence M_adm,k is closed in I_c x C_lin x D_k.

But continuity of Rec_k is not established until the sum over domain histories is uniformly controlled. Thus closedness of the actual-density state space is currently CONDITIONAL, not proved for Balaban.

## 4. Why l-infinity over histories is insufficient

For sector amplitudes a_Sigma >= 0,

sup_Sigma a_Sigma

does not control

sum_Sigma a_Sigma.

Counterexample: N histories with chi_Sigma=1 and sector contribution equal to one. The l-infinity norm is one while the reconstructed sum is N. If N grows with RG depth/cutoff, no cutoff-uniform reconstruction bound follows.

Therefore the old native pointwise norm is only a component norm. A second history/reconstruction norm is mandatory unless the source proves exact disjointness or a uniform finite-overlap theorem.

## 5. Three possible history mechanisms

Let a_Sigma(F) be a nonnegative majorant for the sector contribution before the history sum, including all local component norms needed in the sector.

### A. Exact disjointness

If for every field U

chi_Sigma(U) chi_Sigma'(U)=0
for Sigma != Sigma',

then at most one sector contributes and an l-infinity history norm is enough.

STATUS: NOT VERIFIED. The presence of characteristic functions or a small/large-field partition does not by itself prove this for complete multiscale histories.

### B. Uniform bounded overlap

If

sup_U # {Sigma in Adm_k: chi_Sigma(U) != 0} <= C_hist

with C_hist independent of k, cutoff and volume, then

sup_U sum_Sigma |chi_Sigma(U)| a_Sigma
 <= C_hist sup_Sigma a_Sigma

provided |chi_Sigma|<=1.

STATUS: NOT VERIFIED; no source-derived C_hist has been obtained.

### C. Weighted summability

The robust formulation is to supply source weights W_Sigma >= 0 and operator majorants tau_Sigma for T_Sigma such that

H_hist =
 sup_U sum_{Sigma in Adm_k}
 |chi_Sigma(U)| tau_Sigma W_Sigma
 < infinity

uniformly in k/cutoff/volume,

while the sector data obey
a_Sigma(F) <= R W_Sigma^{-1}.

Then

||Rec_k(c,F)|| <= H_hist R

up to the explicitly controlled exponential/local normalization factors.

This is the natural replacement if histories overlap or branch combinatorially.

STATUS: FORMULATED BUT NO BALABAN HISTORY WEIGHT HAS BEEN EXTRACTED.

## 6. Proposed pre-state norm

Until a source theorem identifies A or B, the candidate state norm must include a history-variation seminorm. Schematically,

||F||_pre =
 max(
   ||F||_pt,
   sup_U sum_{Sigma in Adm_k}
      |chi_Sigma(U)| tau_Sigma a_Sigma(F)
 ).

This is meaningful only after tau_Sigma and the history contribution map are made precise. It is intentionally not declared to be the final Balaban Banach norm yet.

## 7. Native metric verdict

A source-audit entry pinned to Balaban CMP109, printed page 257 / PDF page 9, records the primary definition:

- a localization domain X is a union of a connected finite family of pi_j-cubes;
- d_j(X) is the length of a shortest tree graph contained in X and intersecting all cubes in X, divided by M.

Thus the metric is a shortest-tree/localization-size metric, not diameter.

Direct primary PDF rendering of this page was not available to this run, so this remains SOURCE-EXTRACTED VIA THE AUDIT LEDGER rather than newly direct-primary-verified.

### Why diameter would have failed

For a diameter-only metric in dimension d>=2, the number of connected subsets containing a fixed root and lying in a ball of diameter O(n) can grow like exp(c n^d), not exp(c n). Hence a claim

# {X contains B: diam(X) in [n,n+1)} <= C exp(c n)

is generally false.

The tree-size metric avoids this particular failure because tree length can control the number of cubes.

## 8. Counting theorem for the actual tree-size geometry

Let G be the adjacency graph of source blocks with maximum degree Delta<infinity. Suppose the source tree metric d(X) obeys the geometric coercivity estimate

|X| <= a_0 + a_1 d(X)                                         (TC)

for every connected source localization domain X.

Let A_Delta(m) be any exponential lattice-animal majorant,

# {connected X containing B: |X|=m}
 <= C_A exp(c_A m).

Then for n>=0,

# {X containing B: n <= d(X) < n+1}
 <=
 sum_{m <= a_0+a_1(n+1)} C_A exp(c_A m)
 <= C_count exp(c_count n)

with explicit symbolic choices depending only on
Delta,a_0,a_1,C_A,c_A.

Therefore exponential shell counting follows from:
(i) bounded lattice degree,
(ii) the tree metric,
(iii) TC.

The nontrivial Balaban-specific missing item is TC with constants uniform in j and the exact cube convention.

This is stronger and safer than assuming the counting estimate directly.

## 9. Histories cannot be counted from d_j(X) alone

Even a perfect bound on

# {X: n<=d_j(X)<n+1}

does not control

# {(Sigma,X): n<=d_Sigma(X)<n+1}

because arbitrarily many admissible histories can share the same local polymer X.

Hence polymer counting and history summability are logically distinct certificates.

The strong pair-counting bound requested in the prompt is NOT established. It requires a history multiplicity/weight theorem in addition to tree counting.

## 10. Common analytic-domain problem

CMP119 source transcriptions state that local analytic domains use parameters of the schematic form

alpha_{0,j} = g_j C_0 (log g_j^{-2})^{q_0},
alpha_{1,j} = g_j C_1 (log g_j^{-2})^{q_1}.

The complete effective action retains components generated at historical scales j.

Along the UV trajectory, early-scale g_j can approach zero as the cutoff is removed. Therefore the raw analytic radii alpha_{r,j} may approach zero.

Consequently a naive common raw domain

U_ref(X) subset U_{Sigma,j}^*(X)
for all historical j

need not have a positive cutoff-uniform analytic collar. In fact the available formulas make

delta_* = inf_{j,Sigma} analytic_radius(j,Sigma)

potentially zero.

Thus a common unscaled analytic core has NOT been established and may be the wrong construction.

### Correct repair candidate: normalized analytic charts

For each local source domain introduce a source-faithful chart

Psi_{Sigma,j,X}: U_ref(X) -> U_{Sigma,j}^*(X)

that rescales the small-field coordinates by their native alpha_{0,j},alpha_{1,j}. Define

F_tilde_{Sigma,j,X} = F_{Sigma,j,X} o Psi_{Sigma,j,X}.

A fixed reference H-infinity space is obtained only if one proves uniform bounds on:
- Psi and its inverse on the needed core;
- gauge covariance;
- compatibility with restriction/localization;
- RG chart transitions;
- a positive reference Cauchy collar delta_ref.

No such uniform chart theorem has been extracted yet.

## 11. Running coupling is not just one scalar coordinate

The source recursion is localized:

c_{j-1}(x) = c_j(x) + beta_j(g_{j-1}) phi_j(x),
where c_j(x)=1/g_j^2(x).

Hence a scalar c_j alone does not describe the full coupling profile near domain boundaries unless phi_j and the history geometry are carried elsewhere in the state.

A more faithful augmented state is

(c, q, F, rho),

where
- c is a bulk/reference value of 1/g^2;
- q(x)=c(x)-c is the localized coupling-profile defect;
- F contains the E/R/B/large-field local activities and history data;
- rho is the reconstructed density.

On a deep interior where phi_j=1 and q=0, the source-native scalar direction is

c_j = c_{j-1} - beta_j(g_{j-1}).

Thus, when beta_j>0 in the source convention, c=1/g^2 decreases toward the IR while g increases.

Near boundaries the update also changes q through phi_j/history geometry.

No source theorem has been extracted that proves beta_j itself is independent of every irrelevant activity in the full nonperturbative density. Therefore the safe statement is:
- the displayed coefficient beta_j is written as a function of the coupling in the source recursion;
- the full state update nevertheless depends on the domain/profile data;
- absence of irrelevant-to-marginal feedback beyond that displayed recursion is NOT asserted.

The old ell_W coordinate remains suspended.

## 12. Compatibility conditions: current exact status

| Condition | Mathematical form | Closed/linear? | Source status |
|---|---|---|---|
| admissible histories | encode Sigma in Adm_k | index restriction | exact primary definition not directly verified |
| nesting of Omega/Lambda/S | encode in Adm_k | index restriction | exact source clauses not extracted |
| local support | define F_i intrinsically on local field variables | linear | source locality is supported, exact full dictionary incomplete |
| gauge invariance | F_i(g.U)=F_i(U) | closed linear | supported for local E/B and native spaces |
| Euclidean covariance | pullback equalities | closed linear | supported for indicated regular components |
| normalization | e.g. F_i(1)=0 after the source subtraction convention | closed affine/linear depending convention | component-specific source dictionary incomplete |
| restriction consistency | bounded restriction equalities | closed linear if required | NOT source-verified globally; do not impose by guess |
| overlap relations | compatibility equations or sector weights | depends | NOT extracted |
| density reconstruction | rho=Rec(c,q,F) | nonlinear graph | source form known schematically; uniform convergence missing |
| positivity/normalization of rho | rho>=0, N(rho)=1 | nonlinear/affine closed if topology fixed | requires reconstructed-density topology |

Thus B_adm cannot honestly be declared a closed linear Banach subspace of B_pt yet. The verified linear compatibility layer can be Banach, but the space of actual densities is a nonlinear reconstructed graph.

## 13. RG preservation

At the source-predicate level, the secondary transcription of CMP119 Theorem 1 and CMP122-II Theorem 1 says RT/R preserves the paper's inductive density class while the coupling/constant hypotheses hold.

That is evidence for semantic preservation of the intended compatibility conditions.

However the statement

R(M_adm,k) subset M_adm,k+1

in the newly proposed Banach topology requires:
1. an exact primary definition of Adm_k and all compatibility data;
2. uniform history summability so Rec_k is well-defined/continuous;
3. normalized analytic chart transition bounds;
4. a coupling-profile chart;
5. a source-to-our-state dictionary for every component.

These are not proved. Therefore topology-level RG preservation is BLOCKED.

## 14. Candidate abstract lemmas proved in this iteration

### YM-COMPAT-CLOSED-1
If B is Banach and {C_a:B->Y_a} is any family of bounded linear maps into normed spaces, then
C=intersection_a ker C_a
is a closed linear subspace of B and hence Banach.

### YM-RECON-GRAPH-1
If X,Y are metric spaces and Rec:X->Y is continuous, then its graph is closed in X x Y. If Y is an ordered Banach lattice/function space with closed positive cone and N:Y->R is continuous, then imposing rho>=0 and N(rho)=1 preserves closedness.

### YM-HISTORY-SUP-FAIL-1
There is no cutoff-independent constant C such that
sum_{Sigma=1}^N a_Sigma <= C sup_Sigma a_Sigma
for all N and nonnegative a_Sigma.
Take a_Sigma=1.

### YM-HISTORY-WEIGHT-1
If sector maps satisfy
||Sec_Sigma|| <= tau_Sigma a_Sigma
and
sup_U sum_Sigma |chi_Sigma(U)| tau_Sigma w_Sigma <= H,
while a_Sigma <= R w_Sigma^{-1},
then
sup_U ||sum_Sigma chi_Sigma(U) Sec_Sigma(U)|| <= H R.

### YM-TREE-COUNT-1
Bounded-degree lattice-animal counting plus |X|<=a_0+a_1 d_tree(X) implies exponential shell counting in d_tree.

### YM-DIAMETER-COUNT-FAIL-1
Diameter alone does not imply exponential-in-diameter counting of connected polymers in dimension >=2.

## 15. K_0 decision

A mathematically legitimate quantitative matching tube still requires ALL of:

[c_-,c_+],
a coupling-profile norm/radius,
kappa_*,
a reference analytic chart with delta_*>0,
component radii R_E,R_R,R_B,R_C,...,
polymer/tree constants,
history overlap/summability constants,
and a continuous reconstruction theorem.

These have not all been extracted with cutoff/volume-uniform constants.

Therefore:

STATUS: BLOCKED BEFORE K_0.

No STEP_0 estimate is attempted.


## 16. Primary-source verification ledger for this iteration

Direct primary equation-level access was attempted through the version-of-record/DOI endpoints. CMP119/CMP109 DOI endpoints were inaccessible to the available browser, and the CMP122-II PDF endpoint redirected to a Springer subscription preview. Therefore equation-level claims below are NOT promoted to newly direct-primary-verified status.

| Paper | Location needed | Mathematical content used | Mapping here | Status this run |
|---|---|---|---|---|
| Balaban CMP109 (1987), 109:249-301 | printed p.257 / PDF p.9, definition before (0.24), (0.24)-(0.26) | localization domain and d_j(X) shortest-tree size / M | native polymer metric | SOURCE-EXTRACTED IN SHA-PINNED AUDIT LEDGER; direct primary unavailable |
| Balaban CMP119 (1988), 119:243-285 | (2.17)-(2.18), printed p.257 / PDF p.15 | effective-density sum over admissible domain sequences | reconstruction Rec_k / history index Adm_k | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | (2.23), printed pp.258-259 / PDF pp.16-17 | classical + E/R/B action decomposition | component coordinates F | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | (2.24), Sect.2 | localized recursion for c_j(x)=1/g_j^2(x) | bulk c + profile q | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | (2.25)-(2.29) | local analytic E terms / source analytic domains | local H-infinity charts | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | (2.31), printed p.260 / PDF p.18 | R local exponential tree-metric bound | pointwise native activity norm | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | (2.34)-(2.42), printed pp.260-263 / PDF pp.18-21 | B analytic domains and B local decay | pointwise native activity norm | SECONDARY VISUAL TRANSCRIPTION ONLY |
| same | Theorem 1, printed p.262 / PDF p.20 | RT preserves the paper inductive assumptions under coupling/constant restrictions | semantic RG preservation target | SECONDARY VISUAL TRANSCRIPTION ONLY |
| Balaban CMP122-I (1989), 122:175-202 | (1.70), printed p.192 / PDF p.18 | large-field C local bound | C component | SECONDARY VISUAL TRANSCRIPTION ONLY |
| Balaban CMP122-II (1989), 122:355-392 | Theorem 1, printed p.355 / PDF p.1 | under sufficiently small effective coupling, densities have CMP119 Sect.2 form/bounds | native-class preservation | PAPER ABSTRACT DIRECT; theorem body secondary only |
| same | (1.98)-(1.100), printed p.390 / PDF p.36 | post-R localized remainder bounds | R' component | SECONDARY VISUAL TRANSCRIPTION ONLY |

Paper-level facts directly verified from publisher/repository metadata:
- CMP119 is explicitly about complete effective densities including large-field domains and preservation of their form by renormalization transformations.
- CMP122-II explicitly states in its abstract that it concludes the R-operation bounds and completes the stated ultraviolet-stability proof for four-dimensional pure gauge theories.

Neither abstract supplies the compatibility, history-counting, common-chart, or K_0 constants required here.

## 17. Exact conclusion of Iteration 4

The mathematically legitimate state-space target is now sharper:

1. a closed linear coefficient compatibility layer C_lin;
2. a separate bulk coupling c and coupling-profile q;
3. a history-weighted reconstruction topology making Rec continuous;
4. the nonlinear actual-density graph M_adm;
5. normalized analytic charts with a positive fixed reference collar;
6. tree/polymer counting and independent history entropy control;
7. native RG preservation in this SAME topology.

Items 3-7 are not established with source-uniform constants.

Therefore the existence of a mathematically legitimate quantitative native K_0 has NOT been proved.

STATUS: BLOCKED BEFORE K_0.

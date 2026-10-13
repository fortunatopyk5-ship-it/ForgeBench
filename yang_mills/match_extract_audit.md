# YM-MATCH-EXTRACT-1 source/topology audit — Iteration 3

Scope: only the legitimacy of K_0 and the mathematical preconditions for STEP_0.

## A. Compactness correction

The matching tube is not assumed compact. In the old global chart it is only

K_0={Phi: beta(Phi) in I_0, ||Q Phi||<=R_0},

a closed bounded set when beta,Q are continuous and Phi=beta(Phi)W+QPhi. All STEP_0 bounds are written with suprema; no extremizer is assumed to exist.

## B. Equation-level source extraction status

Important provenance rule:
The qualitative paper-level claims below are corroborated by publisher/metadata sources. Equation-level formulas were cross-checked against a public source-audit repository whose manifest pins local Balaban PDFs by SHA-256 and labels the entries as visually confirmed. In this iteration they are treated as SECONDARY TRANSCRIPTIONS of the primary PDFs, not as direct primary-source verification by this notebook.

| Needed bound | Balaban source statement / extracted formula | Same norm as old global C^p? | Conversion needed? | Status |
|---|---|---:|---|---|
| Density form at scale k | CMP119 (2.18): rho_k is a sum over admissible domain histories with characteristic factor chi_k, T_k, and exp A_k | NO | retain sector labels; reconstructing one global Phi requires a separate theorem | SOURCE FORM EXTRACTED |
| E/R/B split | CMP119 (2.23): A_k=-A(1/g_k^2,U_k)+E_k+R_k+B_k-mathcalE_k | NO | identify classical-action coordinate and component norms | SOURCE FORM EXTRACTED |
| Running coupling coordinate | CMP119 (2.24): 1/g_{j-1}^2(x)=1/g_j^2(x)+beta_j(g_{j-1}) phi_j(x) | NO | dictionary to beta=ell_W(Phi), including sign/normalization | EXPLICIT BUT DICTIONARY OPEN |
| Regular E component | CMP119 (2.25)-(2.29): localized E^(j), analytic extension on U_j^c(X,alpha_0,j,alpha_1,j), paper-I local bound; alpha_i,j proportional to g_j times log powers | NO | exact paper-I inequality + rooted summability + derivative norm on restricted domain | PARTIAL |
| Irrelevant R component | CMP119 (2.31): |R^(j)(X,(U,J))| <= g_j^kappa0 exp(-kappa d_j(X)) | NO | rooted-counting/metric dictionary; derivative/Cauchy bounds if derivative norm used | POINTWISE DECAY EXTRACTED |
| Boundary/large-field B component | CMP119 (2.42): |B^(j)(X,...)| < B0 exp(-kappa d_j(X)) on its regularity domain | NO | sector-indexed norm; metric/counting dictionary | POINTWISE DECAY EXTRACTED |
| Intermediate large-field C | CMP122-I (1.70): |C_k^(n)(X,(U,J))| <= C0 exp(-(1+3 beta)kappa d_m(X)) on stated analytic extension domain | NO | exact hypotheses + sector dictionary + rooted sum | POINTWISE DECAY EXTRACTED |
| Post-R localized remainder | CMP122-II (1.99): |R'^(k)(X)| <= O(1)c1 exp(-(1+beta/2)kappa d_{k,union Y_i}(X)) | NO | post-R dictionary/metric/counting | POINTWISE DECAY EXTRACTED |
| Large-field-disjoint post-R term | CMP122-II (1.100): |R'^(k)(X)| <= exp(-p0(g_k)) exp(-kappa d_k(X)) | NO | preserve exp(-p0(g)) as activity amplitude; do not replace by all-field sup smallness | POINTWISE DECAY EXTRACTED |
| Native-class preservation | CMP122-II Theorem 1: while all effective couplings remain in (0,gamma] for sufficiently small gamma, densities retain the CMP119 Sect.2 form/conditions/bounds | NO | formulate exact native density-space predicate and constants | THEOREM SHAPE EXTRACTED |
| Global all-field remainder radius | ||Q Phi_match||_{alpha,mu,p,rho}<=epsilon_match | N/A | would require global recombination/embedding theorem | NOT OBTAINED |

## C. Dictionary: Balaban coordinate versus crossover coordinate

Balaban's marginal coordinate is naturally the gauge coupling g_j or, in the classical action, the coefficient 1/g_j^2.

The old crossover coordinate beta(Phi)=ell_W(Phi) is a Haar-L2 projection onto a normalized plaquette direction.

No quantitative identity between them has been proved.

Direction check:
toward the IR, asymptotic freedom heuristically means g increases, so the classical coefficient 1/g^2 decreases. Thus if beta(Phi) is intended to represent a Wilson-action coefficient, its direction should be compared with 1/g^2, not with g itself.

Required lemma:
there exist explicit constants/norm conventions such that on the matching class,

beta(Phi) = c_G/g^2 + delta_beta(Phi)

with a rigorously bounded delta_beta, or another source-faithful coordinate should replace ell_W.

Status: UNKNOWN.

## D. Banach-norm verdict

### Old global norm
Mathematically valid as a Banach space, but NOT source-faithful for current Balaban matching estimates.

Why:
1. local analyticity is supplied on restricted U_j^c domains, not all G-link configurations;
2. full densities are decomposed with characteristic/domain histories;
3. large-field suppression is represented through separate activities/domain costs;
4. no theorem controls derivatives of the fully recombined density across sector boundaries in the old global C^p topology.

Therefore no legitimate R_0 in that topology was extracted.

### Corrected candidate
Use first a domain-indexed weighted l^infinity H^infinity activity family with native metric d_Sigma(X) and restricted analytic domains. Keep E,R,B,C sectors separate and keep characteristic histories as combinatorial labels rather than differentiable fields. This is a Banach space and a pointwise source bound A exp(-kappa d_Sigma(X)) feeds it directly. Only afterwards use YM-NATIVE-TAIL-1 plus rooted counting to reach an anchored l^1/KP norm.

## E. Full RG map audit

Write R=L o S o E.

### E: exact fiber integration
Finite-volume differentiation identities are valid:
D E[A]=E[A|V],
D^2 E[A,B]=-Cov(A,B|V).

But this does NOT prove E:B_native->B_E or bounded Frechet derivatives in a volume-uniform activity norm.

Status: FORMULAS PROVED; BANACH MAPPING UNKNOWN.

### S: rescaling/block identification
Expected to be a relabeling with explicit weight/metric change, but its norm depends on the precise native d_Sigma and scale conventions.

Status: EXPLICIT IN PRINCIPLE, NOT YET BOUNDED IN THE CORRECTED NORM.

### L: localization/extraction
The old specification merely named L. Balaban localization is implemented through localized cluster/random-walk expansions and sector bookkeeping. It has not been identified here as a fixed bounded linear operator on the old B, nor as a C^2 map on the corrected native space.

Status: UNKNOWN; THIS BLOCKS FULL D R AND D^2 R.

Correct chain rule is YM-RG-CHAINRULE-1.

## F. STEP_0 bounds — Iteration 3

Because a legitimate K_0 containing the actual matching output has NOT yet been constructed, STEP_0 is not instantiated.

| Certificate | Status | Reason |
|---|---|---|
| f_0^- , f_0^+ | UNKNOWN in old ell_W chart; EXPLICIT BUT UNEVALUATED in native c=1/g^2 chart | CMP119 (2.24) gives the native recursion shape, but no rigorous interval extrema/beta-function constants have been extracted |
| H_0 | UNKNOWN | no native/global K_0 and no bounded localization map |
| A_0 | UNKNOWN | D R for full RG not established in matching topology |
| B_0 | UNKNOWN | same |
| C_0 | UNKNOWN | D^2 R for full RG not established; E covariance identity alone is insufficient |

No toy numbers or dummy constants are used.

## G. Level A / Level B

Level A is proved: STEP_0 needs a finite number of certificates f,H,A,B,C.

Level B is NOT proved: each certificate is currently an infinite-dimensional optimization problem. The interval verifier only checks scalar enclosures after analytic tail/operator bounds have reduced them to trustworthy finite input data.

## H. Adversarial answers

1. Is K_0 nonempty?
   - Abstract old tube: trivially nonempty for suitable I_0,R_0 because beta W belongs to it.
   - A K_0 known to contain the actual Balaban matching output: UNKNOWN.

2. Does uniform R_0<infinity survive cutoff removal?
   - Old global norm: NOT ESTABLISHED.
   - Native Balaban class: UV stability gives native inductive control under small-coupling assumptions, but an explicit scalar R_0 in the new norm still needs extraction/counting constants.

3. Is R_0 small enough for Taylor?
   UNKNOWN.

4. Is localization bounded?
   UNKNOWN in the crossover Banach topology.

5. Does D^2 R exist?
   Fiber E: finite-volume identity yes. Full R: UNKNOWN in the stated topology.

6. Does polymer decomposition converge before KP landing?
   Balaban's small-coupling cluster expansions converge within their own inductive regime; the old global product-Haar polymerization is not thereby justified throughout crossover.

7. Does field supremum destroy large-field suppression?
   It can. Activity/probability suppression is not an all-field pointwise smallness estimate.

8. Is beta=ell_W(Phi) comparable with running coupling?
   UNKNOWN. Natural comparison is with the coefficient 1/g^2, not g.

9. Does R map the chosen space into itself?
   Old global B: UNKNOWN.
   Balaban native inductive class: preserved only in the proved sufficiently-small-coupling regime; this does not establish intermediate-coupling STEP_0.

10. Are the five bounds actually computable?
    Not yet. They are five named infinite-dimensional certificate problems until additional tail/operator reductions are proved.


# Iteration 4 addendum — actual-density state-space audit

## I. Coefficient space versus density space

Correction:
B_pt is a Banach space of local coefficient families. It is not, by itself, the space of Balaban effective densities.

The reconstructed density has schematic form

rho_k = sum_{Sigma in Adm_k} chi_Sigma T_Sigma[exp A_Sigma(c,q,F)].

The exact matching state must therefore contain a reconstruction condition. The mathematically safe representation is the graph

M_adm,k =
{(c,q,F,rho):
  F in C_lin,
  rho=Rec_k(c,q,F),
  rho>=0,
  N(rho)=1}.

Here C_lin is only the closed linear layer of verified compatibility equalities. M_adm,k is generally nonlinear.

## II. Compatibility/gluing status

Admissibility/nesting are encoded in the history index set Adm_k once its exact source definition is extracted.

Verified compatibility constraints that are linear can be packaged as kernels of bounded maps:
- local support/restriction;
- gauge invariance;
- Euclidean covariance where source-required;
- component normalization;
- any actual source-proved overlap/restriction identity.

By YM-COMPAT-CLOSED-1 their intersection is a closed Banach subspace.

However exact global cross-history consistency/overlap rules have not been directly primary-extracted. They are NOT filled in by guess. Thus B_adm as a fully source-faithful closed linear subspace is not yet defined.

## III. History sum audit

The source form sums over admissible histories, and S_j summation is included in T_k.

Therefore

sup_Sigma ||F_Sigma||

is insufficient to control reconstruction.

Three sufficient mechanisms were tested:

A. exact disjointness of characteristic sectors — NOT VERIFIED;
B. cutoff-uniform bounded overlap — NOT VERIFIED;
C. weighted history summability — mathematically sufficient, but no source-derived weights/constants extracted.

A robust target is

H_hist =
sup_U sum_{Sigma in Adm_k}
 |chi_Sigma(U)| tau_Sigma / w_Sigma
< infinity,

with sector majorants a_Sigma<=R/w_Sigma.

No cutoff/volume-uniform H_hist has been proved.

## IV. Native metric audit

The source ledger records CMP109 printed p257 / PDF p9 as defining d_j(X) by

d_j(X) = shortest tree-graph length meeting all pi_j cubes of X / M,

for a connected finite union of localization cubes.

This is a tree-size metric, not diameter.

Direct primary PDF access was not available in this run, so the status remains source-extracted via the audit ledger.

Adversarial result:
diameter alone would fail: in d>=2 there can be exp(c n^d) connected subsets with diameter O(n).

For the tree metric, exponential shell counting follows from

|X| <= a_0+a_1 d_j(X)

plus a standard exponential lattice-animal bound.

Iteration-5 direct CMP109 verification closes this fixed-history geometry:
in d=4,
|X|<=16(d_j(X)+1),
so one may take a_0=a_1=16 in the coarse coercive form. Together with degree-8 face adjacency, DFS counting yields the explicit shell constants recorded in YM-BALABAN-TREE-SHELL-COUNT-1.

History-pair counting remains separate; its source control is the CMP122-II admissible-sequence entropy/large-field-energy mechanism, not this polymer count.

## V. Analytic-domain audit

A single fixed raw domain is not currently justified.

Historical source radii are of the form

alpha_{r,j} ~ g_j (log g_j^{-2})^{q_r}.

As cutoff removal adds earlier UV scales with g_j->0, the infimum of raw radii can approach zero. Thus a naive cutoff-uniform raw collar delta_*>0 may fail.

Required repair:
construct normalized charts

Psi_{Sigma,j,X}: U_ref(X)->U^*_{Sigma,j}(X)

that scale by the native analytic radii and prove uniform chart/inverse/restriction/gauge/RG distortion.

No such chart theorem has been extracted.

## VI. Coupling coordinate audit

The native coupling is a localized profile

c_j(x)=1/g_j^2(x),

not merely one scalar.

The recursion shape

c_{j-1}(x)=c_j(x)+beta_j(g_{j-1}) phi_j(x)

shows that boundary/domain geometry generates a nonconstant profile.

Therefore the augmented native state must contain

(c,q,F,rho),

where c is a bulk/reference scalar and q(x)=c(x)-c is the localized profile defect.

On a deep interior with phi_j=1 the source-native scalar direction is

c_j=c_{j-1}-beta_j(g_{j-1}).

No claim is made that the full nonperturbative marginal update is independent of all irrelevant activities beyond the displayed source recursion.

## VII. K_0 verdict

A legitimate K_0 requires at least

[c_-,c_+],
R_q,
kappa_*,
delta_ref,
R_E,R_R,R_B,R_C,...,
C_count,c_count or equivalent tree constants,
H_hist or exact disjointness/overlap constant,
and a continuous reconstruction theorem.

These have not all been obtained cutoff- and volume-uniformly.

STATUS: BLOCKED BEFORE K_0.

STEP_0 was not attacked.


# Iteration 5 addendum — history reconstruction is source-controlled but not yet Banach-transported

## A. Direct-primary upgrade

CMP119 and CMP122-II article PDFs were directly inspected. The history statements below are no longer secondary transcriptions.

## B. Exact representation correction

CMP119 Eq. (2.18) uses:
- an outer sum over admissible Omega/Lambda domain sequences;
- terminal chi_k(Omega_k);
- T_k, which internally contains the S_j sums;
- A_k depending on Omega/Lambda/S.

Thus the previous schematic notation
sum_Sigma chi_Sigma T_Sigma exp A_Sigma
must be interpreted carefully. There is no independent source chi_Sigma for a complete compressed history.

The correct schematic compressed notation is

rho_k
 =
sum_{H=(Omega,Lambda) in Adm_k^out}
 chi_k(Omega_k)
 K_H[ exp A_H ],

where K_H means the complete T/zeta operator including the internal S history.

## C. Characteristic partition

Raw characteristic decompositions over P,Q,R are exact {0,1} partitions of unity.

But raw P,Q choices are resummed for fixed Omega into zeta(Omega^c), and S is summed in T. Hence compressed history multiplicity is not controlled by chi_k alone.

Final verdict:
- raw level: exact partition;
- compressed Eq. (2.18) level: weighted/resummed T mechanism.

## D. T/operator bound

CMP122-II gives

|(T'_k(X)1)^(-1) T'_k(X)F|
<= e^{3 sup|sigma|} sup|F|,

and later

T'_k(X)1
<= exp[-2(1+beta_0)^(-1)p_0(g_k)].

This provides a genuine local tau-type operator majorant.

## E. History entropy payment

Before CMP122-II Eq. (1.79), the source states that sums over admissible sequences are replaced by

exp{O(1)(MR_j)^(-d)|Z_j|}.

The same estimate contains negative large-field factors; the component form includes p_0(g_j)^2 times linear/tree size and -2p_0(g_j). The subsequent induction proves these costs dominate the history/integration growth, yielding Eq. (1.89).

Therefore:
HISTORY ENTROPY PAYMENT MECHANISM = SOURCE VERIFIED.

No fake scalar p>s is introduced; the source mechanism is genuinely multiscale.

## F. q-profile correction

Primary CMP119 Eq. (2.24) attaches phi_j to Lambda_j and does not sum over phi_j. Once a deterministic cutoff selection is fixed, the localized coupling profile is derived from the bulk couplings and history. It is not an independent physical coordinate.

## G. Remaining reconstruction blocker

The remaining problem is to define a volume-independent local/relative density Banach target D and translate source multiscale suppression into fixed compressed-history weights w_H with

H_0=sum_H tau_H<infinity,
H_1=sum_H tau_H/w_H<infinity

uniform in cutoff, volume and RG depth.

Then YM-HISTORY-RECON-ABSTRACT-1 gives

||Rec(A)||_D<=e^R H_0,

||Rec(A)-Rec(B)||_D<=e^R H_1||A-B||_amb.

This translation is not done.

STATUS: BLOCKED AT YM-HISTORY-RECON-1.


# Iteration 6 addendum — local/relative history norm

The previous requirement for a globally summable complete-history kernel has been dropped as unnecessarily strong.

Direct conclusions now used:
- CMP119 disjoint-component T-factorization;
- CMP122-II connected R/history polymerization and exponentiation;
- CMP109 anchored summability of e^{-lambda d_j(X)}.

Preferred target:
connected relative log-density/activity norm

sup_Delta sum_{X contains Delta}
 e^{mu d_j(X)} ||K_X||.

For source bounds A e^{-Lambda d_j(X)}, the source geometric sum gives the uniform estimate
A C_geo(Lambda-mu).

Thus local H_0 is closed for the source sectors carrying the direct tree-decay estimates, including the improved T' large-field component and regular post-R activity.

Not closed:
the local Lipschitz constant h_1 for the entire compressed outer-history -> connected-activity map, and the fixed normalized analytic chart needed to state that map on one Banach space.

No K_0 is defined in this iteration.
